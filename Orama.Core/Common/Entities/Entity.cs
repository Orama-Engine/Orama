using Orama.Core.Common.Components;

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
    private List<Component> components = new();

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
