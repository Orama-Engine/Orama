
namespace Orama.Core.Common.Entities;

/// <summary>
/// A Container for game logic and components.
/// </summary>
public class BaseEntity
{
    /// <summary> Is this entity enabled? </summary>
    public bool Enabled { get; set; } = true;

    /// <summary> The name of the entity. </summary>
    public string Name { get; set; } = "Entity";

    /// <summary> Called when the entity is enabled. </summary>
    public virtual void Start() { }

    /// <summary> Called every frame. </summary>
    public virtual void Update() { }
}
