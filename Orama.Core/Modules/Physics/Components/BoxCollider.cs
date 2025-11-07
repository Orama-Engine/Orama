using Jitter2.Collision.Shapes;
using Jitter2.LinearMath;
using Orama.Core.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orama.Core.Modules.Physics.Components;

public class BoxCollider : Component
{
    public float Width { get; set; }
    public float Height { get; set; }
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

        JVector size = new JVector(Width, Height, Depth);
        var boxShape = new BoxShape(size);
        Entity.GetComponent<RigidBody>()!.AddShape(boxShape);
    }
}
