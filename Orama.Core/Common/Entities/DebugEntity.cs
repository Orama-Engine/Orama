#if DEBUG
using Orama.Core.Modules;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Rendering.Components;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;

namespace Orama.Core.Common.Entities;

/// <summary>
/// Internal entity useful for debugging systems.
/// </summary>
internal class DebugEntity : BaseEntity
{
    private MeshRenderer? renderer;

    public override void Start()
    {
        Console.WriteLine("Debug entity started.");

        renderer = AddComponent<MeshRenderer>();

        // Simple triangle mesh
        renderer.Mesh = new();
        var mesh = renderer.Mesh;
        mesh.Vertices = new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) };
        mesh.Normals = new Vector3[] { new(1, 0, 0), new(-1, 0, 0), new(-1, 0, 0) };
        mesh.UVs = new Vector2[] { new(0, 0), new(1, 0), new(0, 1) };
        mesh.Indices = new uint[] { 0, 1, 2 };
    }
}

#endif