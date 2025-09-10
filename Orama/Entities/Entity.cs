using System.Numerics;
using System.Collections.ObjectModel;
using Orama.Components;
using Orama.Echo;

namespace Orama.Entities;

/// <summary>
/// The base class for all Entities in a Scene.
/// Holds a collection of Components that contain the logic for the specific Entity.
/// </summary>
public class Entity : ISerializationCallbackReceiver
{
	public string Name { get; set; } = "Entity";

	public Transform Transform { get; set; } = new();

	[Serialize] private readonly List<Component> components = new();
	
	/// <summary> Read-only collection of the Entity's Components </summary>
	[SerializeIgnore] public ReadOnlyCollection<Component> Components => components.AsReadOnly();
	
	public Entity() { }

	/// <summary> Destroys the Entity and its Components. </summary>
	public void Destroy()
	{
		foreach (var component in components.ToList())
			RemoveComponent(component);
	}

	/// <summary> Adds a Component to the Entity. </summary>
	public void AddComponent(Component component)
	{
		components.Add(component);
		component.Entity = this;
	}

	/// <summary> Removes a Component from the Entity. </summary>
	public void RemoveComponent(Component component)
	{
		components.Remove(component);
		component.Entity = null!;
	}
	
	/// <summary> Returns a target Component. </summary>
	public T GetComponent<T>() where T : Component
	{
		return components.OfType<T>().FirstOrDefault();
	}

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize()
	{
		// Reconstruct all component instances
		foreach (var component in components)
			component.Entity = this;
	}
}