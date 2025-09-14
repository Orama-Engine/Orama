using BulletSharp;
using BulletSharp.Math;
using Orama.Modules;
using Orama.Modules.Physics;

namespace Orama.Components;

/// <summary>
/// A physics component that allows an Entity to participate in Bullets physics simulation.
/// Requires a <see cref="Collider"/> component to define the collision shape.
/// </summary>
public class RigidBody : Component
{
	/// <summary>
	/// The collider attached to this rigid body, determines the physical shape.
	/// </summary>
	public Collider Collider { get; set; }
	
	private BulletSharp.RigidBody bulletRb;
	private System.Numerics.Quaternion lastRot;
	private System.Numerics.Vector3 lastPos;
	
	/// <summary>
	/// Determines the Entity's resistance to acceleration when a force is applied.
	/// </summary>
	public float Mass { get; set; } = 1f;
	
	/// <summary>
	/// Force resisting relative motion.
	/// </summary>
	public float Friction { get; set; } = 0.5f;
	
	/// <summary>
	/// Determines how bouncy the Entity is during collisions.
	/// </summary>
	public float Restitution { get; set; }
	
	public override void Start()
	{
		if (ModuleManager.GetModule<PhysicsModule>().World == null)
			throw new InvalidOperationException("PhysicsModule must have its world initialized.");
		
		Collider = Entity.GetComponent<Collider>();
		if (Collider == null)
			throw new Exception("RigidBody requires a Collider component!");
		
		Vector3 inertia = Vector3.Zero;
		
		// Automatically fill inertia
		if (Mass > 0f)
		{
			Collider.Shape.CalculateLocalInertia(Mass, out inertia);
		}

		var startTransform = Matrix.Translation(new BulletSharp.Math.Vector3(Entity.Transform.Position.X, Entity.Transform.Position.Y, Entity.Transform.Position.Z))
		                     * Matrix.RotationQuaternion(new BulletSharp.Math.Quaternion(Entity.Transform.Rotation.X, Entity.Transform.Rotation.Y, Entity.Transform.Rotation.Z, Entity.Transform.Rotation.W));
		var motionState = new DefaultMotionState(startTransform);
		var rbInfo = new RigidBodyConstructionInfo(Mass, motionState, Collider.Shape, inertia);
		bulletRb = new BulletSharp.RigidBody(rbInfo);
		ModuleManager.GetModule<PhysicsModule>().World.AddRigidBody(bulletRb);
	}

	public override void Update()
	{
		if (ModuleManager.GetModule<PhysicsModule>() == null)
			return;
		
		// Check if position changed to sync
		if (Entity.Transform.Position != lastPos || Entity.Transform.Rotation != lastRot)
		{
			SyncToPhysics();
		}
		else
		{
			// Sync Entity transform to Bullet's rigidbody transform.
			var btTransform = bulletRb.MotionState.WorldTransform;
			Entity.Transform.Position = new System.Numerics.Vector3(btTransform.Origin.X, btTransform.Origin.Y, btTransform.Origin.Z);

			var basis = btTransform.Basis;
			// Could probably do this through a helper function to cut down on some lines.
			float trace = basis.M11 + basis.M22 + basis.M33;
			float w, x, y, z;

			if (trace > 0f)
			{
				float s = MathF.Sqrt(trace + 1f) * 2f;
				w = 0.25f * s;
				x = (basis.M23 - basis.M32) / s;
				y = (basis.M31 - basis.M13) / s;
				z = (basis.M12 - basis.M21) / s;
			}
			else if (basis.M11 > basis.M22 && basis.M11 > basis.M33)
			{
				float s = MathF.Sqrt(1f + basis.M11 - basis.M22 - basis.M33) * 2f;
				w = (basis.M23 - basis.M32) / s;
				x = 0.25f * s;
				y = (basis.M12 + basis.M21) / s;
				z = (basis.M31 + basis.M13) / s;
			}
			else if (basis.M22 > basis.M33)
			{
				float s = MathF.Sqrt(1f + basis.M22 - basis.M11 - basis.M33) * 2f;
				w = (basis.M31 - basis.M13) / s;
				x = (basis.M12 + basis.M21) / s;
				y = 0.25f * s;
				z = (basis.M23 + basis.M32) / s;
			}
			else
			{
				float s = MathF.Sqrt(1f + basis.M33 - basis.M11 - basis.M22) * 2f;
				w = (basis.M12 - basis.M21) / s;
				x = (basis.M31 + basis.M13) / s;
				y = (basis.M23 + basis.M32) / s;
				z = 0.25f * s;
			}
			Entity.Transform.Rotation = new System.Numerics.Quaternion(x, y, z, w);
		}
		
		// Store last transforms to check for position changes
		lastPos = Entity.Transform.Position;
		lastRot = Entity.Transform.Rotation;
	}
	
	/// <summary>
	/// Synchronises the Entity's transform with Bullet's RigidBody.
	/// </summary>
	public void SyncToPhysics()
	{
		var pos = Entity.Transform.Position;
		var rot = Entity.Transform.Rotation;

		var btTransform = Matrix.RotationQuaternion(new Quaternion(rot.X, rot.Y, rot.Z, rot.W)) *
		                  Matrix.Translation(new Vector3(pos.X, pos.Y, pos.Z));

		bulletRb.WorldTransform = btTransform;

		if (bulletRb.MotionState != null)
			bulletRb.MotionState.WorldTransform = btTransform;
	}
}