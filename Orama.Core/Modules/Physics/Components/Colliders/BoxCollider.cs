
namespace Orama.Core.Modules.Physics.Components.Colliders;

public class BoxCollider : Collider
{
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }

    public BoxCollider(float width, float height, float depth) => (Width, Height, Depth) = (width, height, depth);

    public override void Start()
    {
        var rb = Entity.GetComponent<RigidBody>();
        if (rb != null) shapeId = rb.AddBoxCollider(Width, Height, Depth);
    }
}
