
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
}
