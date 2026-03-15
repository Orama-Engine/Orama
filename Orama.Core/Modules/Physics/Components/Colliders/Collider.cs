using Orama.Core.Common.Components;

namespace Orama.Core.Modules.Physics.Components.Colliders;

public abstract class Collider : Component
{
    public override void Start()
    {
        var rb = Entity.GetComponent<RigidBody>();
        if (rb != null) rb.AddShape(CreateShape());
    }

    protected abstract ICollisionShape CreateShape();
}
