
using Orama.Core.Common.Entities;

namespace Orama.Core.Common.Components;

/// <summary>
/// A Container for reusable modular logic.
/// </summary>
public class BaseComponent
{
    /// <summary> Is this component enabled? </summary>
    public bool Enabled { get; set; } = true;

    /// <summary> The entity this component is attached to. </summary>
    public BaseEntity Entity { get; internal set; } = null!;

    /// <summary> Called when the component is enabled. </summary>
    public virtual void Start() { }

    /// <summary> Called every frame. </summary>
    public virtual void Update() { }
}
