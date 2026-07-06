using Orama.Common.Utility;
using Orama.Rendering;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;
using System.Runtime.InteropServices;

namespace Orama.VirtualReality.OpenXR.Bindings;

/// <summary>
/// Managed bindings for <see cref="Instance"/>.
/// </summary>
internal class OpenXRInstance : OpenXRBinding
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate Result pfnGetD3D11GraphicsRequirementsKHR(Instance instance, ulong systemId, GraphicsRequirementsD3D11KHR* req);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate Result pfnGetVulkanGraphicsRequirementsKHR(Instance instance, ulong systemId, GraphicsRequirementsVulkanKHR* req);

    /// <summary> The native <see cref="Instance"/>. </summary>
    public Instance Native { get; }

    /// <summary> The list of extensions that this instance supports. </summary>
    public IReadOnlyList<string> Extensions => extensions;

    /// <summary> The name of the Virtual Reality headset. </summary>
    public string DeviceName { get; private set; }

    /// <summary> The ID of the current system. </summary>
    public ulong SystemID
    {
        get
        {
            ulong systemID = 0;

            SystemGetInfo systemInfo = new()
            {
                Type = StructureType.SystemGetInfo,
                FormFactor = FormFactor.HeadMountedDisplay // TODO: Support handhelds?
            };

            OpenXR.GetSystem(Native, ref systemInfo, ref systemID).VerifySuccess();

            return systemID;
        }
    }

    private List<string> extensions = new();

    public OpenXRInstance(XR openXR) : base(openXR)
    {
        extensions = GetRequiredExtensions() ?? new List<string>();
        IntPtr extensionsPtr = SilkMarshal.StringArrayToPtr((IReadOnlyList<string>)Extensions);

        ApplicationInfo appInfo = new ApplicationInfo()
        {
            ApiVersion = new Version64(1, 0, 0)
        };

        unsafe
        {
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
            OpenXR.CreateInstance(ref createInfo, ref instance).VerifySuccess();

            Native = instance;

            SystemProperties properties = new()
            {
                Type = StructureType.SystemProperties
            };

            OpenXR.GetSystemProperties(instance, SystemID, ref properties).VerifySuccess();

            DeviceName = Marshal.PtrToStringAnsi((IntPtr)properties.SystemName) ?? "Unknown";

            CheckGraphicsRequirements();
        }
    }

    private unsafe List<string>? GetRequiredExtensions()
    {
        List<string> extensions = new();

        string requiredExtension = Renderer.Backend switch
        {
            RendererBackend.Vulkan => "XR_KHR_vulkan_enable",
            RendererBackend.Direct3D11 => "XR_KHR_D3D11_enable",
            _ => throw new NotSupportedException("Unsupported renderer backend for OpenXR")
        };

        extensions.Add(requiredExtension);

        uint propCount = 0;
        OpenXR.EnumerateInstanceExtensionProperties((byte*)null, 0, &propCount, null).VerifySuccess();

        Span<ExtensionProperties> props = new ExtensionProperties[propCount];
        for (int i = 0; i < props.Length; i++)
        {
            props[i].Type = StructureType.ExtensionProperties;
            props[i].Next = null;
        }

        OpenXR.EnumerateInstanceExtensionProperties((byte*)null, propCount, &propCount, props).VerifySuccess();

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

        return extensions;
    }

    // This needs to run for a valid instance
    private unsafe void CheckGraphicsRequirements()
    {
        switch (Renderer.Backend)
        {
            case RendererBackend.Direct3D11:
                {
                    PfnVoidFunction fnPtr = new();
                    OpenXR.GetInstanceProcAddr(Native, "xrGetD3D11GraphicsRequirementsKHR", ref fnPtr).VerifySuccess();

                    var fn = Marshal.GetDelegateForFunctionPointer<pfnGetD3D11GraphicsRequirementsKHR>((IntPtr)fnPtr.Handle);

                    GraphicsRequirementsD3D11KHR requirements = new()
                    {
                        Type = StructureType.GraphicsRequirementsD3D11Khr
                    };

                    fn(Native, SystemID, &requirements);

                    break;
                }

            case RendererBackend.Vulkan:
                {
                    PfnVoidFunction fnPtr = new();
                    OpenXR.GetInstanceProcAddr(Native, "xrGetVulkanGraphicsRequirementsKHR", ref fnPtr).VerifySuccess();

                    var fn = Marshal.GetDelegateForFunctionPointer<pfnGetVulkanGraphicsRequirementsKHR>((IntPtr)fnPtr.Handle);

                    GraphicsRequirementsVulkanKHR requirements = new()
                    {
                        Type = StructureType.GraphicsRequirementsVulkanKhr
                    };

                    fn(Native, SystemID, &requirements);

                    break;
                }

            default:
                throw new NotSupportedException("Unsupported renderer backend for OpenXR");
        }
    }
}
