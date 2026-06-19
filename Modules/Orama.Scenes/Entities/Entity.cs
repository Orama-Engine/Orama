using Orama.Core.Common.Components;
using Orama.Modules;
using Orama.Scenes;
using Orama.Serialization.Attributes;
using System.Reflection;

namespace Orama.Core.Common.Entities;

/// <summary>
/// A Container for game logic and components.
/// </summary>
public class Entity
{
    /// <summary> Is this entity enabled? </summary>
    public bool Enabled
    {
        get;
        set
        {
            field = value;

            foreach (var component in Components)
                component.Enabled = value;
        }
    } = true;

    /// <summary> The name of the entity. </summary>
    public string Name { get; set; } = "Entity";

    /// <summary> The transform of the entity. </summary>
    public Transform Transform { get; set; } = new();

    /// <summary> The components attached to the entity. </summary>
    public IReadOnlyList<Component> Components => components;

    [AlwaysSerialize]
    private List<Component> components = new();

    /// <summary> Initializes a new instance of the <see cref="Entity"/> class. </summary>
    public Entity()
    {
        Transform.Entity = this;
        ModuleManager.GetModule<SceneModule>()?.CurrentScene.Entities.Add(this);

        foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (field.FieldType.IsAssignableTo(typeof(Component)) && field.GetCustomAttributes(typeof(ImplicitComponent), false).Length > 0)
            {
                var component = (Component?)field.GetValue(this);
                if (component == null)
                {
                    component = (Component)Activator.CreateInstance(field.FieldType)!;
                    field.SetValue(this, component);
                }

                AddComponent(component);
            }
        }

        foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (property.PropertyType.IsAssignableTo(typeof(Component)) && property.GetCustomAttributes(typeof(ImplicitComponent), false).Length > 0 && property.CanWrite)
            {
                var component = (Component?)property.GetValue(this);
                if (component == null)
                {
                    component = (Component)Activator.CreateInstance(property.PropertyType)!;
                    property.SetValue(this, component);
                }

                AddComponent(component);
            }
        }
    }

    /// <summary> Called when the entity is enabled. </summary>
    /// <remarks> The base implementation starts all components. </remarks>
    public virtual void Start()
    {
        foreach (var component in Components)
        {
            if (component.Enabled)
                component.Start();
        }
    }

    /// <summary> Called every frame. </summary>
    /// <remarks> The base implementation updates all components. </remarks>
    public virtual void Update()
    {
        foreach (var component in Components)
        {
            if (component.Enabled)
                component.Update();
        }
    }

    /// <summary> Destroys the specified entity and releases any associated resources. </summary>
    public void Destroy()
    {
        Enabled = false;

        foreach (var component in Components.ToList()) // ToList for enumeration issues
        {
            component.Enabled = false;
            component.Destroy();
        }

        var scene = ModuleManager.GetModule<SceneModule>()?.CurrentScene;
        scene?.Entities.Remove(this);

        components.Clear();
        Transform = null!;
    }

    #region Component Methods
    /// <summary> Adds a component to the entity. </summary>
    public Component AddComponent(Component component)
    {
        component.Entity = this;
        components.Add(component);

        return component;
    }

    /// <summary> Gets a component of type T from the entity. </summary>
    public T? GetComponent<T>() where T : Component => (T?)components.Find(c => c is T);

    /// <summary> Adds a new component of type T to the entity. </summary>
    public T AddComponent<T>() where T : Component, new() => (T)AddComponent(new T());

    /// <summary> Gets a component of type T from the entity or adds a new one if it doesn't exist. </summary>
    public T GetOrAddComponent<T>() where T : Component, new() => GetComponent<T>() ?? AddComponent<T>();

    /// <summary> Removes a component from the entity. </summary>
    public void RemoveComponent(Component component) => components.Remove(component);
    #endregion
}
