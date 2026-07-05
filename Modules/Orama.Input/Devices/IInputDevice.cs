
namespace Orama.Input.Devices;

/// <summary>
/// Base interface for all Input Devices. (i.e., Controllers, Keyboards, etc.)
/// </summary>
public interface IInputDevice
{
    /// <summary> Runs once per <see cref="InputModule"/> update. </summary>
    void Update() { }
}