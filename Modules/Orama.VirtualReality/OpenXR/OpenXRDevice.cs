using Orama.Common.Utility;
using Orama.Rendering;
using Silk.NET.Core;
using Silk.NET.OpenXR;
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

    /// <summary> The OpenXR API. </summary>
    public XR OpenXR { get; private set; } = null!;

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
                if (GraphicsBinding == null || Instance == null)
                    return null;

                SessionCreateInfo createInfo = new()
                {
                    Type = StructureType.SessionCreateInfo,
                    SystemId = Instance.SystemID,
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


            EngineConsole.Log(Instance.SystemID.ToString());
            EngineConsole.Log(Instance.Native.ToString() ?? "NULL");

            if (Instance == null)
                return false;

            if (!CheckGraphicsRequirements())
                return false;

            EngineConsole.Log(Session?.Handle.ToString() ?? "NULL");

            return true;
        }
    }

    private unsafe bool CheckGraphicsRequirements()
    {
        if (Instance == null)
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

                    res = fn(Instance.Native, Instance.SystemID, &requirements);
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

                    res = fn(Instance.Native, Instance.SystemID, &requirements);
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