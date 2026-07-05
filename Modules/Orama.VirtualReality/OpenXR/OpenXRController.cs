
namespace Orama.VirtualReality.OpenXR;

internal class OpenXRController : VirtualRealityController
{
    /// <inheritdoc/>
    public override bool IsButtonPressed(Button button)
    {
        return false;
    }

    /// <inheritdoc/>
    public override void Update() { }
}
