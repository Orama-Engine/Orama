using Orama.Common;
using Orama.Input;
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

    // Kinda hacky, maybe move to VirtualRealityDevice?
    public bool IsValid => OpenXR != null && Instance != null && Session != null;

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

        Name = Instance.DeviceName;

        // Input
        OpenXRInput.Initialize(OpenXR, Instance);
        OpenXRInput.Attach(OpenXR, Session);

        var leftHand = new OpenXRController(this, VirtualRealityController.HandType.Left);
        var rightHand = new OpenXRController(this, VirtualRealityController.HandType.Right);

        var inputModule = ModuleManager.GetModule<InputModule>();
        inputModule?.AddDevice(leftHand);
        inputModule?.AddDevice(rightHand);
    }

    /// <inheritdoc/>
    public override void Update()
    {
        base.Update();

        if (!IsValid)
            return;

        OpenXRInput.Sync(OpenXR, Session);
        Session.PollEvents();
        Session.SubmitBlank();
    }
}
