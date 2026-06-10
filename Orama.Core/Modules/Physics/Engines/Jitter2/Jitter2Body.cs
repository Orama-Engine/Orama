using Jitter2.Collision.Shapes;
using Jitter2.Dynamics;
using Jitter2.LinearMath;
using Orama.Core.Common.Entities;
using Orama.Core.Modules.Physics.Components.Colliders;
using Orama.Math;

namespace Orama.Core.Modules.Physics.Engines.Jitter2;

/// <summary>
/// Represents a Jitter2 physics body within the physics world.
/// </summary>
public class Jitter2Body : IPhysicsBody
{
    public Entity? Owner { get; init; }

    internal readonly RigidBody? body;
    private readonly Dictionary<RigidBody, Jitter2Body> bodyMap;
    private readonly Dictionary<int, RigidBodyShape> shapes = new();
    private int nextId = 0;

    internal Jitter2Body(RigidBody body, Dictionary<RigidBody, Jitter2Body> bodyMap)
    {
        this.body = body;
        this.bodyMap = bodyMap;

        body.BeginCollide += (arb) =>
        {
            var other = arb.Body1 == body ? arb.Body2 : arb.Body1;
            if (bodyMap.TryGetValue(other, out var jb))
                OnCollisionEnter?.Invoke(jb);
        };

        body.EndCollide += (arb) =>
        {
            var other = arb.Body1 == body ? arb.Body2 : arb.Body1;
            if (bodyMap.TryGetValue(other, out var jb))
                OnCollisionExit?.Invoke(jb);
        };
    }

    /// <inheritdoc/>
    public bool IsStatic
    {
        get => body?.IsStatic ?? false;
        set { if (body != null) body.IsStatic = value; }
    }

    /// <inheritdoc/>
    public float Mass
    {
        get => body != null ? body.Mass : 0f;
        set
        { 
            if (body != null)
                if (body.IsStatic != true)
                    body.SetMassInertia(value);
        }
    }

    /// <inheritdoc/>
    public float Friction
    {
        get => body != null ? body.Friction : 0f;
        set { if (body != null) body.Friction = value; }
    }

    /// <inheritdoc/>
    public bool AffectedByGravity
    {
        get => body?.AffectedByGravity ?? false;
        set { if (body != null) body.AffectedByGravity = value; }
    }

    /// <inheritdoc/>
    public float Restitution
    {
        get => body != null ? body.Restitution : 0f;
        set { if (body != null) body.Restitution = value; }
    }

    /// <inheritdoc/>
    public Vector3 AngularVelocity
    {
        get => body != null ? new Vector3(body.AngularVelocity.X, body.AngularVelocity.Y, body.AngularVelocity.Z) : Vector3.Zero;
        set { if (body != null) body.AngularVelocity = new JVector(value.X, value.Y, value.Z); }
    }

    /// <inheritdoc/>
    public Vector2 Damping
    {
        get => body != null ? new Vector2(body.Damping.linear, body.Damping.angular) : Vector2.Zero;
        set { if (body != null) body.Damping = (value.X, value.Y); }
    }

    /// <inheritdoc/>
    public Vector3 Velocity
    {
        get => body != null ? new Vector3(body.Velocity.X, body.Velocity.Y, body.Velocity.Z) : Vector3.Zero;
        set { if (body != null) body.Velocity = new JVector(value.X, value.Y, value.Z); }
    }

    /// <inheritdoc/>
    public Vector3 Position
    {
        get => body != null ? new Vector3(body.Position.X, body.Position.Y, body.Position.Z) : Vector3.Zero;
        set { if (body != null) body.Position = new JVector(value.X, value.Y, value.Z); }
    }

    /// <inheritdoc/>
    public Quaternion Orientation
    {
        get => body != null ? new Quaternion(body.Orientation.X, body.Orientation.Y, body.Orientation.Z, body.Orientation.W) : Quaternion.Identity;
        set { if (body != null) body.Orientation = new JQuaternion(value.X, value.Y, value.Z, value.W); }
    }

    /// <inheritdoc/>
    public int AddBoxCollider(float width, float height, float depth)
    {
        if (body == null) return -1;

        RigidBodyShape shape = new BoxShape(width * 2, height * 2, depth * 2);
        body.AddShape(shape);
        shapes[nextId] = shape;
        return nextId++;
    }

    /// <inheritdoc/>
    public int AddSphereCollider(float radius)
    {
        if (body == null) return -1;

        RigidBodyShape shape = new SphereShape(radius);
        body.AddShape(shape);
        shapes[nextId] = shape;
        return nextId++;
    }

    /// <inheritdoc/>
    public void RemoveCollider(int id)
    {
        if (body != null && shapes.TryGetValue(id, out var shape))
        {
            body.RemoveShape(shape);
            shapes.Remove(id);
        }
    }

    /// <inheritdoc/>
    public void AddForce(Vector3 force)
    {
        if (body != null)
            body.AddForce(new JVector(force.X, force.Y, force.Z));
    }

    /// <inheritdoc/>
    public event Action<IPhysicsBody>? OnCollisionEnter;

    /// <inheritdoc/>
    public event Action<IPhysicsBody>? OnCollisionExit;
}
