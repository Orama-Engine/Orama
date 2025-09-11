namespace Orama.Physics;

/// <summary>
/// Specifies the type of collision shape used by a <see cref="Collider"/>.
/// </summary>
public enum ColliderType
{
	/// <summary>
	/// A box-shaped collider
	/// </summary>
	Box,
	
	/// <summary>
	/// A sphere-shaped collider.
	/// </summary>
	Sphere,
	
	/// <summary>
	/// A capsule-shaped collider.
	/// </summary>
	Capsule,
}