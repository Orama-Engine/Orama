#if DEBUG
using Orama.Core.Modules;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Rendering.Resources;
using System.Numerics;

namespace Orama.Core.Common.Entities;

/// <summary>
/// Internal entity useful for debugging systems.
/// </summary>
internal class DebugEntity : BaseEntity
{
    private Mesh mesh = new();

    public override void Start()
    {
        Console.WriteLine("Debug entity started.");

        // Simple triangle mesh
        mesh.Vertices = new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) };
        mesh.Normals = new Vector3[] { new(1, 0, 0), new(-1, 0, 0), new(-1, 0, 0) };
        mesh.UVs = new Vector2[] { new(0, 0), new(1, 0), new(0, 1) };
        mesh.Indices = new uint[] { 0, 1, 2 };
    }

    public override void Update()
    {
        ModuleManager.GetOrRegisterModule<RenderingModule>().RenderMesh(mesh);
    }
}

#endif