
namespace Orama.Core.Modules.Physics.Components.Colliders;

public class SphereCollider : Collider
{
    public float Radius { get; set; }

    public SphereCollider(float radius) => Radius = radius;

    public override void Start()
    {
        var rb = Entity.GetComponent<RigidBody>();
        if (rb != null) shapeId = rb.AddSphereCollider(Radius);
    }
}
