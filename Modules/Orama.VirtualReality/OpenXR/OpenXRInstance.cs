using Orama.Common.Utility;
using Orama.Rendering;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;

namespace Orama.VirtualReality.OpenXR;

/// <summary>
/// Managed bindings for <see cref="Silk.NET.OpenXR.Instance"/>.
/// </summary>
internal class OpenXRInstance : OpenXRBinding
{
    /// <summary> The native <see cref="Silk.NET.OpenXR.Instance"/>. </summary>
    public Instance Native { get; }

    /// <summary> The list of extensions that this instance supports. </summary>
    public IReadOnlyList<string> Extensions => extensions;

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

            Result result = OpenXR.GetSystem(Native, ref systemInfo, ref systemID);

            if (result != Result.Success)
                throw new Exception($"Failed to get system ID: {result}");

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
            Result res = OpenXR.CreateInstance(ref createInfo, ref instance);
            if (res != Result.Success)
                throw new Exception($"Failed to create OpenXR instance: {res}");

            Native = instance;
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

        return extensions;
    }
}
