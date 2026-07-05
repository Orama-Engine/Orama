using Orama.Common.Utility;
using Orama.Rendering;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Orama.VirtualReality.OpenXR;

/// <summary>
/// A <see cref="VirtualRealityDevice"/> that uses OpenXR.
/// </summary>
internal class OpenXRDevice : VirtualRealityDevice
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate Result pfnGetD3D11GraphicsRequirementsKHR(Instance instance, ulong systemId, GraphicsRequirementsD3D11KHR* req);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate Result pfnGetVulkanGraphicsRequirementsKHR(Instance instance, ulong systemId, GraphicsRequirementsVulkanKHR* req);

    public const string XR_KHR_VULKAN_ENABLE_EXTENSION_NAME = "XR_KHR_vulkan_enable";
    public const string XR_KHR_D3D11_ENABLE_EXTENSION_NAME = "XR_KHR_D3D11_enable";

    /// <summary> The OpenXR API. </summary>
    public XR OpenXR { get; private set; } = null!;

    /// <summary> This device's OpenXR System ID. </summary>
    public ulong? SystemID
    {
        get
        {
            if (field != null)
                return field;

            ulong systemID = 0;

            SystemGetInfo systemInfo = new()
            {
                Type = StructureType.SystemGetInfo,
                FormFactor = FormFactor.HeadMountedDisplay // TODO: Support handhelds?
            };

            if (Instance == null)
                return null;

            Result result = OpenXR.GetSystem(Instance.Value, ref systemInfo, ref systemID);

            if (result != Result.Success)
            {
                EngineConsole.Warning($"Failed to get OpenXR System: {result}");
                return null;
            }

            field = systemID;
            return field;
        }
    }

    /// <summary> This device's OpenXR Instance. </summary>
    public Instance? Instance
    {
        get
        {
            if (field != null)
                return field;

            unsafe
            {
                string requiredExtension = Renderer.Backend switch
                {
                    RendererBackend.Vulkan => XR_KHR_VULKAN_ENABLE_EXTENSION_NAME,
                    RendererBackend.Direct3D11 => XR_KHR_D3D11_ENABLE_EXTENSION_NAME,
                    _ => throw new NotSupportedException("Unsupported renderer backend for OpenXR")
                };

                uint propCount = 0;
                OpenXR.EnumerateInstanceExtensionProperties((byte*)null, 0, &propCount, null);

                Span<ExtensionProperties> props = new ExtensionProperties[propCount];
                for (int i = 0; i < props.Length; i++)
                {
                    props[i].Type = StructureType.ExtensionProperties;
                    props[i].Next = null;
                }
                OpenXR.EnumerateInstanceExtensionProperties((byte*)null, propCount, &propCount, props);

                bool found = false;
                for (int i = 0; i < props.Length; i++)
                {
                    fixed (void* namePtr = props[i].ExtensionName)
                    {
                        string name = SilkMarshal.PtrToString((IntPtr)namePtr)!;
                        if (name == requiredExtension)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    EngineConsole.Warning($"Required OpenXR extension not supported: {requiredExtension}");
                    return null;
                }

                var extensions = new List<string> { requiredExtension };
                IntPtr extensionsPtr = SilkMarshal.StringArrayToPtr(extensions);

                ApplicationInfo appInfo = new ApplicationInfo()
                {
                    ApiVersion = new Version64(1, 0, 0)
                };

                Span<byte> appNameSpan = new Span<byte>(appInfo.ApplicationName, 128);
                Span<byte> engineNameSpan = new Span<byte>(appInfo.EngineName, 128);

                SilkMarshal.StringIntoSpan("Orama", appNameSpan);
                SilkMarshal.StringIntoSpan("None", engineNameSpan);

                InstanceCreateInfo createInfo = new InstanceCreateInfo
                {
                    Type = StructureType.InstanceCreateInfo,
                    ApplicationInfo = appInfo,
                    EnabledExtensionCount = (uint)extensions.Count,
                    EnabledExtensionNames = (byte**)extensionsPtr,
                    Next = null
                };

                Instance instance = new();

                Result result = OpenXR.CreateInstance(ref createInfo, ref instance);

                if (result != Result.Success)
                {
                    EngineConsole.Warning($"Failed to create OpenXR Instance: {result}");
                    return null;
                }

                field = instance;
                return field;
            }
        }
    }

    /// <summary> This device's OpenXR Session. </summary>
    public Session? Session
    {
        get
        {
            if (field != null)
                return field;

            unsafe
            {
                if (GraphicsBinding == null || SystemID == null || Instance == null)
                    return null;

                SessionCreateInfo createInfo = new()
                {
                    Type = StructureType.SessionCreateInfo,
                    SystemId = SystemID.Value,
                    Next = GraphicsBinding
                };

                Session session = new();
                Result result = OpenXR.CreateSession(Instance.Value, &createInfo, &session);

                if (result != Result.Success)
                {
                    EngineConsole.Warning($"Failed to create OpenXR Session: {result}");
                    return null;
                }

                field = session;
                return field;
            }
        }
    }

    private static GraphicsBindingD3D11KHR d3d11Binding;
    private static GraphicsBindingVulkanKHR vulkanBinding;

    /// <summary> Pointer to the current renderers Graphics Binding. </summary>
    public static unsafe void* GraphicsBinding
    {
        get
        {
            if (field != null)
                return field;

            unsafe
            {
                switch (Renderer.Backend)
                {
                    case RendererBackend.Vulkan:
                        vulkanBinding = new GraphicsBindingVulkanKHR
                        {
                            Type = StructureType.GraphicsBindingVulkanKhr,

                            Instance = new VkHandle(Renderer.Veldrid.GraphicsDevice.GetVulkanInfo().Instance),
                            PhysicalDevice = new VkHandle(Renderer.Veldrid.GraphicsDevice.GetVulkanInfo().PhysicalDevice),
                            Device = new VkHandle(Renderer.Veldrid.GraphicsDevice.GetVulkanInfo().Device),

                            QueueFamilyIndex = Renderer.Veldrid.GraphicsDevice.GetVulkanInfo().GraphicsQueueFamilyIndex,
                            QueueIndex = 0
                        };
                        field = Unsafe.AsPointer(ref vulkanBinding);
                        break;

                    case RendererBackend.Direct3D11:
                        d3d11Binding = new GraphicsBindingD3D11KHR()
                        {
                            Type = StructureType.GraphicsBindingD3D11Khr,
                            Device = (void*)Renderer.Veldrid.GraphicsDevice.GetD3D11Info().Device,
                        };
                        field = Unsafe.AsPointer(ref d3d11Binding);
                        break;

                    default:
                        break;

                }
            }

            return field;
        }
    }

    /// <inheritdoc/>
    public override bool TryInitialize()
    {
        unsafe
        {
            OpenXR = XR.GetApi();

            EngineConsole.Log(SystemID?.ToString() ?? "NULL");
            EngineConsole.Log(Instance?.Handle.ToString() ?? "NULL");

            if (Instance == null || SystemID == null)
                return false;

            if (!CheckGraphicsRequirements())
                return false;

            EngineConsole.Log(Session?.Handle.ToString() ?? "NULL");

            return true;
        }
    }

    private unsafe bool CheckGraphicsRequirements()
    {
        if (Instance == null || SystemID == null)
            return false;

        switch (Renderer.Backend)
        {
            case RendererBackend.Direct3D11:
                {
                    PfnVoidFunction fnPtr = new();
                    Result res = OpenXR.GetInstanceProcAddr(Instance.Value, "xrGetD3D11GraphicsRequirementsKHR", ref fnPtr);
                    if (res != Result.Success)
                    {
                        EngineConsole.Warning($"Failed to get xrGetD3D11GraphicsRequirementsKHR: {res}");
                        return false;
                    }

                    var fn = Marshal.GetDelegateForFunctionPointer<pfnGetD3D11GraphicsRequirementsKHR>((IntPtr)fnPtr.Handle);

                    GraphicsRequirementsD3D11KHR requirements = new()
                    {
                        Type = StructureType.GraphicsRequirementsD3D11Khr
                    };

                    res = fn(Instance.Value, SystemID.Value, &requirements);
                    if (res != Result.Success)
                    {
                        EngineConsole.Warning($"xrGetD3D11GraphicsRequirementsKHR failed: {res}");
                        return false;
                    }

                    EngineConsole.Log($"D3D11 min feature level: {requirements.MinFeatureLevel}, adapter LUID: {requirements.AdapterLuid}");
                    return true;
                }

            case RendererBackend.Vulkan:
                {
                    PfnVoidFunction fnPtr = new();
                    Result res = OpenXR.GetInstanceProcAddr(Instance.Value, "xrGetVulkanGraphicsRequirementsKHR", ref fnPtr);
                    if (res != Result.Success)
                    {
                        EngineConsole.Warning($"Failed to get xrGetVulkanGraphicsRequirementsKHR: {res}");
                        return false;
                    }

                    var fn = Marshal.GetDelegateForFunctionPointer<pfnGetVulkanGraphicsRequirementsKHR>((IntPtr)fnPtr.Handle);

                    GraphicsRequirementsVulkanKHR requirements = new()
                    {
                        Type = StructureType.GraphicsRequirementsVulkanKhr
                    };

                    res = fn(Instance.Value, SystemID.Value, &requirements);
                    if (res != Result.Success)
                    {
                        EngineConsole.Warning($"xrGetVulkanGraphicsRequirementsKHR failed: {res}");
                        return false;
                    }

                    EngineConsole.Log($"Vulkan API version range: {requirements.MinApiVersionSupported} - {requirements.MaxApiVersionSupported}");
                    return true;
                }

            default:
                EngineConsole.Warning("Unsupported renderer backend for OpenXR");
                return false;
        }
    }
}