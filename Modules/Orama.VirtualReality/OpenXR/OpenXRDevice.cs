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

            Result result = OpenXR.GetSystem(Instance.Native, ref systemInfo, ref systemID);

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
    public OpenXRInstance Instance { get; private set; } = null!;

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
                    Next = (void*)GraphicsBinding.Native
                };

                Session session = new();
                Result result = OpenXR.CreateSession(Instance.Native, &createInfo, &session);

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

    /// <summary> Current renderers Graphics Binding. </summary>
    public static OpenXRGraphicsBinding GraphicsBinding { get; private set; } = null!;

    /// <inheritdoc/>
    public override bool TryInitialize()
    {
        unsafe
        {
            OpenXR = XR.GetApi();
            GraphicsBinding = new OpenXRGraphicsBinding(OpenXR, Renderer.Backend);
            Instance = new OpenXRInstance(OpenXR);


            EngineConsole.Log(SystemID?.ToString() ?? "NULL");
            EngineConsole.Log(Instance.Native.ToString() ?? "NULL");

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
                    Result res = OpenXR.GetInstanceProcAddr(Instance.Native, "xrGetD3D11GraphicsRequirementsKHR", ref fnPtr);
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

                    res = fn(Instance.Native, SystemID.Value, &requirements);
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
                    Result res = OpenXR.GetInstanceProcAddr(Instance.Native, "xrGetVulkanGraphicsRequirementsKHR", ref fnPtr);
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

                    res = fn(Instance.Native, SystemID.Value, &requirements);
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