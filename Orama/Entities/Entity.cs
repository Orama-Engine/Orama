using System.Numerics;
using System.Collections.ObjectModel;
using Orama.Components;
using Orama.Echo;
using System.Reflection;

namespace Orama.Entities;

/// <summary>
/// The base class for all Entities in a Scene.
/// Holds a collection of Components that contain the logic for the specific Entity.
/// </summary>
public class Entity : ISerializationCallbackReceiver
{
	/// <summary> The name of the Entity. </summary>
	public string Name { get; set; } = "Entity";

	public Transform Transform { get; set; } = new();

	[Serialize] private readonly List<Component> components = new();
	
	/// <summary> Read-only collection of the Entity's Components </summary>
	[SerializeIgnore] public ReadOnlyCollection<Component> Components => components.AsReadOnly();
	
	public Entity()
	{
		AddImplicitComponents();
	}

	public Entity(string name)
	{
		Name = name;
		AddImplicitComponents();
	}


	/// <summary> Runs when the Entity is created. </summary>
	public virtual void Start()
	{
		foreach (var component in components)
			component.Start();
	}

	/// <summary> Runs every game tick. </summary>
	public virtual void Update()
	{
		foreach (var component in components)
			component.Update();
	}

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

	private void AddImplicitComponents()
	{
		// Reflect and add all implicit components
		foreach (var field in GetType().GetFields())
		{
			var attribute = field.GetCustomAttribute<ImplicitComponentAttribute>();
			if (attribute != null)
			{
				// Create the component
				var component = (Component)Activator.CreateInstance(field.FieldType)!;
				component.Entity = this;
				components.Add(component);

				// Set the field
				field.SetValue(this, component);
			}
		}

		foreach (var property in GetType().GetProperties())
		{
			var attribute = property.GetCustomAttribute<ImplicitComponentAttribute>();
			if (attribute != null)
			{
				// Create the component
				var component = (Component)Activator.CreateInstance(property.PropertyType)!;
				component.Entity = this;
				components.Add(component);

				property.SetValue(this, component);
			}
		}
	}
}