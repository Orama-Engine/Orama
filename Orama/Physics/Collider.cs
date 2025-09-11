using BulletSharp;
using BulletSharp.Math;
using Orama.Components;

namespace Orama.Physics;

/// <summary>
/// A component that defines the physical collision shape of an Entity.
/// </summary>
public class Collider : Component
{
	/// <summary>
	/// The type of <see cref="ColliderType"/> to use (Box, Sphere, Capsule).
	/// </summary>
	public ColliderType Type { get; set; } = ColliderType.Box;
	
	/// <summary>
	/// The collision shape used by the physics engine.
	/// </summary>
	public CollisionShape Shape { get; set; }
	
	/// <summary>
	/// Half extents for box colliders, scaled by the Entity's transform.
	/// </summary>
	public Vector3 HalfExtents { get; set; } = new Vector3(0.5f);
	
	/// <summary>
	/// Radius for sphere and capsule colliders, scaled by the Entity's transform.
	/// </summary>
	public float Radius { get; set; } = 0.5f;
	
	/// <summary>
	/// Height for capsule colliders, scaled by the Entity's transform.
	/// </summary>
	public float Height { get; set; } = 1f;

	public override void Start()
	{
		switch(Type)
		{
			case ColliderType.Box:
				Shape = new BoxShape(
					new Vector3(
						HalfExtents.X * Entity.Transform.Scale.X,
						HalfExtents.Y * Entity.Transform.Scale.Y,
						HalfExtents.Z * Entity.Transform.Scale.Z
					)
				);
				break;
			case ColliderType.Sphere:
				float maxScale = Math.Max(Entity.Transform.Scale.X, Math.Max(Entity.Transform.Scale.Y, Entity.Transform.Scale.Z));
				Shape = new SphereShape(Radius * maxScale);
				break;
			case ColliderType.Capsule:
				float horizontalScale = Math.Max(Entity.Transform.Scale.X, Entity.Transform.Scale.Z);
				Shape = new CapsuleShape(Radius * horizontalScale, Height * Entity.Transform.Scale.Y);
				break;
		}
	}
}