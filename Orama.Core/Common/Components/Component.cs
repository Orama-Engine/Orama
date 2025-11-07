
using Orama.Core.Common.Entities;

namespace Orama.Core.Common.Components;

/// <summary>
/// A Container for reusable modular logic.
/// </summary>
public class Component
{
    /// <summary> Is this component enabled? </summary>
    public bool Enabled { get; set; } = true;

    /// <summary> The entity this component is attached to. </summary>
    public Entity Entity { get; internal set; } = null!;

    /// <summary> Called when the component is enabled. </summary>
    public virtual void Start() { }

    /// <summary> Called every frame. </summary>
    public virtual void Update() { }

    /// <summary> Called when the component is destroyed. </summary>
    public virtual void Destroy()
    {
        Enabled = false;
        Entity.RemoveComponent(this);
        Entity = null!;
    }
}
