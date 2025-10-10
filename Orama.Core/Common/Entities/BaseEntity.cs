using Orama.Core.Common.Components;

namespace Orama.Core.Common.Entities;

/// <summary>
/// A Container for game logic and components.
/// </summary>
public class BaseEntity
{
    private bool enabled = true;

    /// <summary> Is this entity enabled? </summary>
    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            foreach (var component in Components)
            {
                component.Enabled = value;
            }
        }
    }

    /// <summary> The name of the entity. </summary>
    public string Name { get; set; } = "Entity";

    /// <summary> The components of the entity. </summary>
    public List<BaseComponent> Components { get; set; } = new();

    /// <summary> Called when the entity is enabled. </summary>
    public virtual void Start()
    {
        foreach (var component in Components)
        {
            if (component.Enabled)
                component.Start();
        }
    }

    /// <summary> Called every frame. </summary>
    public virtual void Update()
    {
        foreach (var component in Components)
        {
            if (component.Enabled)
                component.Update();
        }
    }
}
