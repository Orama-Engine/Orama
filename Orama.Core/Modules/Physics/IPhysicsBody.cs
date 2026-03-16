using Orama.Core.Modules.Physics.Components.Colliders;
using Orama.Math;

namespace Orama.Core.Modules.Physics;

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

    /// <summary> Adds a box collider to the physics body.</summary>
    public int AddBoxCollider(float width, float height, float depth);

    /// <summary> Adds a sphere collider to the physics body.</summary>
    public int AddSphereCollider(float radius);

    /// <summary> Removes a collider from the physics body.</summary>
    /// <param name="id"> The id linked to the collider to detach from the body. </param>
    void RemoveCollider(int id);

    /// <summary> Applies a force to the physics body.</summary>
    /// <param name="force">The force to apply to the body.</param>
    void AddForce(Vector3 force);
}