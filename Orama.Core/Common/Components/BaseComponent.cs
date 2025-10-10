
namespace Orama.Core.Common.Components;

/// <summary>
/// A Container for reusable modular logic.
/// </summary>
public class BaseComponent
{
    /// <summary> Is this component enabled? </summary>
    public bool Enabled { get; set; } = true;

    /// <summary> Called when the component is enabled. </summary>
    public virtual void Start() { }

    /// <summary> Called every frame. </summary>
    public virtual void Update() { }
}
