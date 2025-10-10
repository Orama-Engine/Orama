
namespace Orama.Core.Modules;

/// <summary>
/// Base class for all modules.
/// </summary>
public class BaseModule
{
    /// <summary> The dependencies of the module. </summary>
    public virtual HashSet<Type> Dependencies => new();

    /// <summary> Runs once at the start of the module. </summary>
    public virtual void Initialize() { }

    /// <summary> Runs every frame. </summary>
    public virtual void Update() { }

    /// <summary> Runs once at the end of the module. </summary>
    public virtual void Dispose() { }
}
