
namespace Orama.Core.Modules.Physics.Components.Colliders;

public class BoxCollider : Collider, ICollisionShape
{
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }

    public BoxCollider(float width, float height, float depth) => (Width, Height, Depth) = (width, height, depth);

    protected override ICollisionShape CreateShape() => this;
}
