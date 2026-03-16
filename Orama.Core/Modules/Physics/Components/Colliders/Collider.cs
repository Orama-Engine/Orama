using Orama.Core.Common.Components;

namespace Orama.Core.Modules.Physics.Components.Colliders;

public class Collider : Component
{
    public override void Start()
    {
        var rb = Entity.GetComponent<RigidBody>();
        if (rb != null) rb.AddShape(CreateShape());
    }

    protected virtual ICollisionShape CreateShape()
    {
        throw new NotImplementedException();
    }
}
