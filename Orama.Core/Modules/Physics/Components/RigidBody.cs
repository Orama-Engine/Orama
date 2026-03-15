using Orama.Core.Common.Components;
using Orama.Math;

namespace Orama.Core.Modules.Physics.Components;

/// <summary>
/// Represents a physics rigid body that enables simulation of physical interactions for an entity.
/// </summary>
public class RigidBody : Component, IPhysicsBody
{
    private IPhysicsBody? body;

    public RigidBody()
    {
        var physics = ModuleManager.GetModule<PhysicsModule>();
        body = physics?.World?.CreateBody();
    }

    /// <inheritdoc/>
    public bool IsStatic
    {
        get => body?.IsStatic ?? false;
        set { if (body != null) body.IsStatic = value; }
    }

    /// <inheritdoc/>
    public bool AffectedByGravity
    {
        get => body?.AffectedByGravity ?? false;
        set { if (body != null) body.AffectedByGravity = value; }
    }

    /// <inheritdoc/>
    public float Mass
    {
        get => body?.Mass ?? 0f;
        set { if (body != null) body.Mass = value; }
    }

    /// <inheritdoc/>
    public float Friction
    {
        get => body?.Friction ?? 0f;
        set { if (body != null) body.Friction = value; }
    }

    /// <inheritdoc/>
    public float Restitution
    {
        get => body?.Restitution ?? 0f;
        set { if (body != null) body.Restitution = value; }
    }

    /// <inheritdoc/>
    public Vector3 Position
    {
        get => body?.Position ?? Vector3.Zero;
        set { if (body != null) body.Position = value; }
    }

    /// <inheritdoc/>
    public Quaternion Orientation
    {
        get => body?.Orientation ?? new Quaternion(0, 0, 0, 1);
        set { if (body != null) body.Orientation = value; }
    }

    /// <inheritdoc/>
    public Vector3 Velocity
    {
        get => body?.Velocity ?? Vector3.Zero;
        set { if (body != null) body.Velocity = value; }
    }

    /// <inheritdoc/>
    public Vector3 AngularVelocity
    {
        get => body?.AngularVelocity ?? Vector3.Zero;
        set { if (body != null) body.AngularVelocity = value; }
    }

    /// <inheritdoc/>
    public Vector2 Damping
    {
        get => body?.Damping ?? Vector2.Zero;
        set { if (body != null) body.Damping = value; }
    }

    public override void Start()
    {
        if (body != null)
            body.Position = Entity.Transform.Position;
    }

    public override void Update()
    {
        if (body != null)
        {
            Entity.Transform.Position = body.Position;
            Entity.Transform.Rotation = body.Orientation;
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        var physics = ModuleManager.GetModule<PhysicsModule>();
        if (body != null) physics?.World?.DestroyBody(body);
    }

    /// <summary> Adds a collision shape to the physics body.</summary>
    /// <param name="shape"> The collision shape to attach to the body. </param>
    public void AddShape(ICollisionShape shape)
    {
        body?.AddShape(shape);
        if (IsStatic)
            Orientation = Entity.Transform.Rotation;
    }

    /// <summary> Removes a collision shape from the physics body.</summary>
    /// <param name="shape"> The collision shape to detach from the body. </param>
    public void RemoveShape(ICollisionShape shape) => body?.RemoveShape(shape);

    /// <summary> Applies a force to the physics body.</summary>
    /// <param name="force">The force to apply to the body.</param>
    public void AddForce(Vector3 force) => body?.AddForce(force);
}
