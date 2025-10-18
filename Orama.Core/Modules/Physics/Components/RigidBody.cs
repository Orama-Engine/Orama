using Jitter2.Collision.Shapes;
using Jitter2.DataStructures;
using Jitter2.LinearMath;
using Orama.Core.Common.Components;
using Orama.Core.Modules;
using Orama.Core.Modules.Physics;

namespace Orama.Core.Modules.Physics.Components;

/// <summary>
/// Represents a physics rigid body that enables simulation of physical interactions for an entity.
/// </summary>
public class RigidBody : Component
{
    Jitter2.Dynamics.RigidBody? body;
    public bool IsStatic /// <summary> Whether the rigid body is static or dynamic. </summary>
    {
        get => body?.IsStatic ?? false;
        set
        {
            if (body != null)
            {
                body.IsStatic = value;
            }
        }
    }
    public float Mass /// <summary> Mass of the rigid body. </summary>
    {
        get => body != null ? body.Mass : 0f;
        set
        {
            if (body != null)
            {
                if (body.IsStatic != true)
                    body.SetMassInertia(value);
            }
        }
    }
    public float Friction /// <summary> Friction of the rigid body. </summary>
    {
        get => body != null ? body.Friction : 0f;
        set
        {
            if (body != null)
            {
                body.Friction = value;
            }
        }
    }
    public bool AffectedByGravity /// <summary> Whether the rigid body is affected by gravity. </summary>
    {
        get => body?.AffectedByGravity ?? false;
        set
        {
            if (body != null)
            {
                body.AffectedByGravity = value;
            }
        }
    }
    public float Restitution /// <summary> Restitution of the rigid body. </summary>
    {
        get => body != null ? body.Restitution : 0f;
        set
        {
            if (body != null)
            {
                body.Restitution = value;
            }
        }
    }
    public float AngularVelocity /// <summary> Angular velocity of the rigid body. </summary>
    {
        get => body != null ? body.AngularVelocity.Length() : 0f;
        set
        {
            if (body != null)
            {
                if (body.AngularVelocity.LengthSquared() > 0)
                {
                    var normalized = JVector.Normalize(body.AngularVelocity);
                    body.AngularVelocity = JVector.Multiply(normalized, value);
                }
                else
                {
                    body.AngularVelocity = new JVector(value, 0, 0);
                }
            }
        }
    }
    public Math.Vector2 Damping /// <summary> Linear and angular damping of the rigid body. </summary>
    {
        get => body != null ? new Math.Vector2(body.Damping.linear, body.Damping.angular) : Math.Vector2.Zero;
        set
        {
            if (body != null)
            {
                body.Damping = (value.X, value.Y);
            }
        }
    }
    public Math.Vector3 Velocity
    {
        get => body != null ? new Math.Vector3(body.Velocity.X, body.Velocity.Y, body.Velocity.Z) : Math.Vector3.Zero;
        set
        {
            if (body != null)
            {
                body.Velocity = new JVector(value.X, value.Y, value.Z);
            }
        }
    }
    

    public ReadOnlyList<RigidBodyShape> Shapes /// <summary> Gets the list of shapes attached to the rigid body. </summary>
    {
        get => (ReadOnlyList<RigidBodyShape>)(body?.Shapes ?? null!);
    }

    public RigidBody(RigidBodyShape shape, bool IsStatic)
    {
        var physics = ModuleManager.GetModule<PhysicsModule>();
        if (physics != null)
        {
            body = physics.World.CreateRigidBody();
            body.IsStatic = IsStatic;
            AddShape(shape); // Attach initial shape
        }
    }

    public override void Start()
    {
        // TODO: Set initial position and rotation
        body?.Position = new JVector(Entity.Transform.Position.X, Entity.Transform.Position.Y, Entity.Transform.Position.Z);
        // Setting initial rotation currently breaks things, commented out temporarily.
        // body?.Orientation = new JQuaternion(Entity.Transform.Rotation.X, Entity.Transform.Rotation.Y, Entity.Transform.Rotation.Z, Entity.Transform.Rotation.W);
    }

    /// <summary> Adds a shape to the rigid body, enabling collision detection and physical interactions for the specified shape. </summary>
    /// <param name="shape">The shape to add to the rigid body. Cannot be null.</param>
    public void AddShape(RigidBodyShape shape)
    {
        body?.AddShape(shape);
    }

    /// <summary> Removes the specified shape from the rigid body, detaching it from physics calculations. </summary>
    /// <param name="shape">The shape to remove from the rigid body. Cannot be null.</param>

    public void RemoveShape(RigidBodyShape shape)
    {
        body?.RemoveShape(shape);
    }

    /// <summary> Applies a force vector to the physics body, affecting its motion according to the force applied. </summary>
    /// <param name="force">The force to apply to the physics body.</param>
    public void AddForce(Math.Vector3 force)
    {
        if (body != null)
        {
            body.AddForce(new JVector(force.X, force.Y, force.Z));
        }
    }

    public override void Update()
    {
        if (body != null)
        {
            Entity.Transform.Position = new Math.Vector3(body.Position.X, body.Position.Y, body.Position.Z);
            Entity.Transform.Rotation = new Math.Quaternion(body.Orientation.X, body.Orientation.Y, body.Orientation.Z, body.Orientation.W);
        }
    }
}
