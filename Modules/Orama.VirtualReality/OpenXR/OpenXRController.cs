
namespace Orama.VirtualReality.OpenXR;

internal class OpenXRController : VirtualRealityController
{
    private OpenXRDevice owner;

    public OpenXRController(OpenXRDevice owner, HandType handness)
    {
        this.owner = owner;
        Handness = handness;
    }

    /// <inheritdoc/>
    public override bool IsButtonPressed(Button button)
    {
        // obviously hacky
        var path = button switch
        {
            Button.ActionUp => OpenXRInput.InputPath.ActionDown,
            Button.ActionDown => OpenXRInput.InputPath.ActionUp,
            Button.System => OpenXRInput.InputPath.System,
            _ => OpenXRInput.InputPath.ActionDown,
        };

        return OpenXRInput.GetBool(owner.OpenXR, owner.Session, (int)Handness, path);
    }

    /// <inheritdoc/>
    public override void Update()
    {
        TriggerPressedAmount = OpenXRInput.GetFloat(owner.OpenXR, owner.Session, (int)Handness, OpenXRInput.InputPath.TriggerValue);
        GripPressedAmount = OpenXRInput.GetFloat(owner.OpenXR, owner.Session, (int)Handness, OpenXRInput.InputPath.GripValue);
    }
}
