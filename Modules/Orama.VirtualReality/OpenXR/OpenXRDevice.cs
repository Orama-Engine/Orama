using Orama.Rendering;
using Orama.VirtualReality.OpenXR.Bindings;
using Silk.NET.OpenXR;

namespace Orama.VirtualReality.OpenXR;

/// <summary>
/// A <see cref="VirtualRealityDevice"/> that uses OpenXR.
/// </summary>
internal class OpenXRDevice : VirtualRealityDevice
{
    /// <summary> Current renderers Graphics Binding. </summary>
    public static OpenXRGraphicsBinding GraphicsBinding { get; private set; } = null!;

    /// <summary> The OpenXR API. </summary>
    public XR OpenXR { get; private set; } = null!;

    /// <summary> This device's OpenXR Instance. </summary>
    public OpenXRInstance Instance { get; private set; } = null!;

    /// <summary> This device's OpenXR Session. </summary>
    public OpenXRSession Session { get; private set; } = null!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        OpenXR = XR.GetApi();
        GraphicsBinding = new OpenXRGraphicsBinding(OpenXR, Renderer.Backend);
        Instance = new OpenXRInstance(OpenXR);
        Session = new OpenXRSession(OpenXR, GraphicsBinding, Instance);
    }
}
