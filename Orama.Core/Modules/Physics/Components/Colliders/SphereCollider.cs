
namespace Orama.Core.Modules.Physics.Components.Colliders;

public class SphereCollider : Collider, ICollisionShape
{
    public float Radius { get; set; }

    public SphereCollider(float radius) => Radius = radius;

    protected override ICollisionShape CreateShape() => this;
}
