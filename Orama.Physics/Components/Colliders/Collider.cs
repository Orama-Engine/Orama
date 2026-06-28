using Orama.Scenes.Components;

namespace Orama.Physics.Components.Colliders;

public class Collider : Component
{
    protected int shapeId = -1;

    public override void Destroy()
    {
        var rb = Entity.GetComponent<RigidBody>();
        if (rb != null && shapeId != -1) rb.RemoveCollider(shapeId);
    }
}
