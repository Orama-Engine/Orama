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

            EngineConsole.Log(Session?.Handle.ToString() ?? "NULL");

            return true;
        }
    }
}