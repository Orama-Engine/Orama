using Jitter2.Collision.Shapes;
using Jitter2.LinearMath;
using Orama.Core.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orama.Core.Modules.Physics.Components;

/// <summary>
/// Represents a sphere-shaped collider component that defines the physical boundaries of an entity for collision
/// detection.
/// </summary>
public class SphereCollider : Component
{
    /// <summary> Radius of the sphere collider. </summary>
    public float Radius { get; set; }

    public SphereCollider(float radius)
    {
        Radius = radius;
    }

    public override void Start()
    {
        if (Entity.GetComponent<RigidBody>() == null)
            return;

        var sphereShape = new SphereShape(Radius);
        Entity.GetComponent<RigidBody>()!.AddShape(sphereShape);
    }
}
