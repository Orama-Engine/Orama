using Jitter2.Collision.Shapes;

namespace Orama.Core.Modules.Physics.Engines.Jitter2;

/// <summary>
/// Represents a Jitter2 collision shape attached to a physics body.
/// </summary>
public class Jitter2CollisionShape : ICollisionShape
{
    /// <summary> Wrapped Jitter2 shape. </summary>
    public RigidBodyShape Shape { get; }

    /// <summary> Wraps the given Jitter2 shape. </summary>
    /// <param name="shape"> The Jitter2 shape to wrap. </param>
    internal Jitter2CollisionShape(RigidBodyShape shape) => Shape = shape;
}
