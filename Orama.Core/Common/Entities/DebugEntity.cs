#if DEBUG
using Orama.Core.Common.Utility;
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

    public override void Start()
    {
        base.Start();

        EngineOutput.Log("Debug entity started.");

        var mesh = Application.ResourceProvider.GetResource<Mesh>("Assets/PrimitiveCube.fbx");

        Renderer.Mesh = mesh;
        Renderer.Mesh?.Material = Material.Default;
    }
}
#endif
