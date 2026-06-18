#if DEBUG
using Orama.Core.Common.Utility;
using Orama.Core.Modules.Physics.Components;
using Orama.Core.Modules.Physics.Components.Colliders;
using Orama.Core.Modules.Rendering.Components;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;

namespace Orama.Core.Common.Entities;

/// <summary>
/// Internal entity useful for debugging systems.
/// </summary>
[Entity("debug_entity")]
public class DebugEntity : Entity
{
    [ImplicitComponent]
    public MeshRenderer Renderer { get; private set; } = null!;

    [ImplicitComponent]
    public RigidBody RigidBody { get; private set; } = null!;

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();

        BoxCollider col = (BoxCollider)AddComponent(new BoxCollider(Transform.Scale));
        col.Start();

        RigidBody.IsStatic = true;

        EngineConsole.Log("Debug entity started.");
    }
}
#endif
