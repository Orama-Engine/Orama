using Orama.Components;
using Orama.Entities;
using Orama.Resources.ResourceLibrary;
using Orama.Echo;
using System.Text;
using Orama.Modules;
using Orama.Modules.Scenes;

namespace Orama.Resources;

/// <summary>
/// Represents a Scene containing and managing a collection of Entities.
/// </summary>
public class Scene : IResource<Scene>
{
	/// <summary> Creates an empty Scene. </summary>
	public Scene() { }

	[Serialize]
    private HashSet<Entity> allEntities = new();
    
    /// <summary> The number of registered Entities. </summary>
    public int Count => allEntities.Count;

    /// <summary> Enumerates all currently registered Entities. </summary>
    public IEnumerable<Entity> AllEntities => allEntities;

    /// <summary> Returns whether this Scene is completely devoid of any Entities. </summary>
    public bool IsEmpty => !AllEntities.Any();

    /// <summary> Registers an Entity to the Scene. </summary> <param name="entity"></param>
    public void Add(Entity entity)
    {
	    allEntities.Add(entity);
    }

    /// <summary> Unregisters an Entity from the Scene. </summary> <param name="entity"></param>
    public void Remove(Entity entity)
    {
	    allEntities.Remove(entity);
    }

    /// <summary> Unregisters all Entities from the Scene. </summary>
    public void Clear()
    {
	    allEntities.Clear();
    }

	public Scene Deserialize(Stream stream)
	{
		var reader = new StreamReader(stream);
		var tag = EchoObject.ReadFromString(reader.ReadToEnd());
		return Serializer.Deserialize<Scene>(tag);
	}

	public void Serialize(Stream stream)
	{
		var tag = Serializer.Serialize(this);
		var serializedString = tag.WriteToString();
		using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
		writer.Write(serializedString);
		writer.Flush();
	}
}