
namespace Orama.VirtualReality;

/// <summary>
/// Base class for all Virtual Reality devices.
/// </summary>
/// <remarks>
/// "Virtual Reality Device" is defined as any device that provides 3D input and visualization for a world to the user.
/// </remarks>
public abstract class VirtualRealityDevice
{
    public string Name { get; set; } = "Unknown VR Device";
    public abstract void Initialize();
    public virtual void Update() { }
}
