namespace Orama.Resources;

/// <summary>
/// Represents a Scene containing and managing a collection of Entities.
/// </summary>
public class Scene
{
    /// <summary> Creates an empty Scene. </summary>
    public Scene() { }

    public HashSet<Entity> Entities = new HashSet<Entity>();
    
    /// <summary> The number of registered Entities. </summary>
    public int Count => Entities.Count;
    
    /// <summary> Enumerates all currently registered Entities. </summary>
    public IEnumerable<Entity> AllEntities => Entities;

    /// <summary> Returns whether this Scene is completely devoid of any Entities. </summary>
    public bool IsEmpty => !AllEntities.Any();

    /// <summary> Registers an Entity to the Scene. </summary> <param name="entity"></param>
    public void Add(Entity entity)
    {
    }

    /// <summary> Unregisters an Entity from the Scene. </summary> <param name="entity"></param>
    public void Remove(Entity entity)
    {
    }

    /// <summary> Unregisters all Entities from the Scene. </summary>
    public void Clear()
    {
    }
}