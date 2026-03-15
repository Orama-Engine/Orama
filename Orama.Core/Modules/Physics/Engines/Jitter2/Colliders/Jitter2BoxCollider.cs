using Jitter2.Collision.Shapes;
using Jitter2.LinearMath;
using Orama.Core.Modules.Physics.Components.Colliders;

namespace Orama.Core.Modules.Physics.Engines.Jitter2.Colliders;

public class Jitter2BoxCollider : BoxCollider
{
    public Jitter2BoxCollider(float width, float height, float depth) : base(width, height, depth) { }

    protected override ICollisionShape CreateShape()
        => new Jitter2CollisionShape(new BoxShape(new JVector(Width * 2, Height * 2, Depth * 2)));
}
