using Jitter2.Collision.Shapes;
using Jitter2.LinearMath;
using Orama.Core.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orama.Core.Modules.Physics.Components;

/// <summary>
/// Represents a box-shaped collider component that defines the physical boundaries of an entity for collision
/// detection.
/// </summary>
public class BoxCollider : Component
{
    /// <summary> Width of the box collider. </summary>
    public float Width { get; set; }

    /// <summary> Height of the box collider. </summary>
    public float Height { get; set; }

    /// <summary> Depth of the box collider. </summary>
    public float Depth { get; set; }

    public BoxCollider(float width, float height, float depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
    }

    public override void Start()
    {
        if (Entity.GetComponent<RigidBody>() == null)
            return;

        JVector size = new JVector(Width * 2, Height * 2, Depth * 2);
        var boxShape = new BoxShape(size);
        Entity.GetComponent<RigidBody>()!.AddShape(boxShape);
    }
}
