#if DEBUG
using Orama.Core.Common.Utility;
using Orama.Core.Modules.Rendering.Components;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;

namespace Orama.Core.Common.Entities;

/// <summary>
/// Internal entity useful for debugging systems.
/// </summary>
internal class DebugEntity : Entity
{
    [ImplicitComponent] private MeshRenderer renderer = null!;

    public override void Start()
    {
        base.Start();

        EngineOutput.Log("Debug entity started.");

        renderer.Mesh = new();
        var mesh = renderer.Mesh;

        // Cube vertices (8 corners)
        Vector3[] vertices = new Vector3[]
        {
            new(-0.5f, -0.5f, -0.5f), // 0
            new( 0.5f, -0.5f, -0.5f), // 1
            new( 0.5f,  0.5f, -0.5f), // 2
            new(-0.5f,  0.5f, -0.5f), // 3
            new(-0.5f, -0.5f,  0.5f), // 4
            new( 0.5f, -0.5f,  0.5f), // 5
            new( 0.5f,  0.5f,  0.5f), // 6
            new(-0.5f,  0.5f,  0.5f), // 7
        };

        // Cube normals per face (we’ll assign per vertex for simplicity)
        Vector3[] normals = new Vector3[]
        {
            // Front
            new(0, 0, -1), new(0, 0, -1), new(0, 0, -1), new(0, 0, -1),
            // Back
            new(0, 0, 1), new(0, 0, 1), new(0, 0, 1), new(0, 0, 1)
        };

        // Cube UVs (one for each vertex)
        Vector2[] uvs = new Vector2[]
        {
            new(0, 0), new(1, 0), new(1, 1), new(0, 1),
            new(0, 0), new(1, 0), new(1, 1), new(0, 1)
        };

        // Indices for 12 triangles (6 faces)
        uint[] indices = new uint[]
        {
            // Front
            0, 1, 2, 2, 3, 0,
            // Right
            1, 5, 6, 6, 2, 1,
            // Back
            5, 4, 7, 7, 6, 5,
            // Left
            4, 0, 3, 3, 7, 4,
            // Top
            3, 2, 6, 6, 7, 3,
            // Bottom
            4, 5, 1, 1, 0, 4
        };

        mesh.Vertices = vertices;
        mesh.Normals = normals;
        mesh.UVs = uvs;
        mesh.Indices = indices;
        mesh.Material = Material.Default;
    }
}
#endif
