
namespace Orama.Input.Devices;

/// <summary>
/// Base class for all Input Devices. (i.e., Controllers, Keyboards, etc.)
/// </summary>
public abstract class InputDevice
{
    /// <summary> Runs once per <see cref="InputModule"/> update. </summary>
    internal virtual void Update() { }
}