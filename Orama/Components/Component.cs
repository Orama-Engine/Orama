using Orama.Resources;

namespace Orama.Components;

/// <summary>
/// Represents a modular component that can be attached to an Entity.
/// </summary>
public class Component
{
	/// <summary> The component's parent Entity. </summary>
	public Entity? Entity { get; internal set; }
	
	/// <summary> Runs once on component initialisation. </summary>
	public virtual void Start() { }
	
	/// <summary> Runs each frame. </summary>
	public virtual void Update() { }
}