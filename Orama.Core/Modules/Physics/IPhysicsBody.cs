using Orama.Math;

namespace Orama.Core.Modules.Physics;

/// <summary>
/// Represents a collision shape attached to a physics body.
/// </summary>
public interface ICollisionShape { }

/// <summary>
/// Represents a physics body within the simulation.
/// </summary>
public interface IPhysicsBody
{
    /// <summary> Whether the physics body is static or dynamic. </summary>
    bool IsStatic { get; set; }

    /// <summary> Whether the physics body is affected by gravity. </summary>
    bool AffectedByGravity { get; set; }

    /// <summary> Mass of the physics body. </summary>
    float Mass { get; set; }

    /// <summary> Friction of the physics body. </summary>
    float Friction { get; set; }

    /// <summary> Restitution of the physics body. </summary>
    float Restitution { get; set; }

    /// <summary> Position of the physics body. </summary>
    Vector3 Position { get; set; }

    /// <summary> Orientation of the physics body. </summary>
    Quaternion Orientation { get; set; }

    /// <summary> Velocity of the physics body. </summary>
    Vector3 Velocity { get; set; }

    /// <summary> Angular velocity of the physics body. </summary>
    Vector3 AngularVelocity { get; set; }

    /// <summary> Linear (X) and angular (Y) damping of the physics body. </summary>
    Vector2 Damping { get; set; }

    /// <summary> Adds a collision shape to the physics body.</summary>
    /// <param name="shape"> The collision shape to attach to the body. </param>
    void AddShape(ICollisionShape shape);

    /// <summary> Removes a collision shape from the physics body.</summary>
    /// <param name="shape"> The collision shape to detach from the body. </param>
    void RemoveShape(ICollisionShape shape);

    /// <summary> Applies a force to the physics body.</summary>
    /// <param name="force">The force to apply to the body.</param>
    void AddForce(Vector3 force);
}