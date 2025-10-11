using Orama.Rendering.Resources;
using Orama.Math;

namespace Orama.Core.Modules.Rendering.Resources;

/// <summary>
/// A Collection of vertices, indices, normals and UVs that form a renderable object.
/// </summary>
public class Mesh
{
    /// <summary> The material to use when rendering this mesh. </summary>
    public Material Material { get; set; } = Material.Default;

    public Vector3[] Vertices { get; set; } = new Vector3[0];
    public uint[] Indices { get; set; } = new uint[0];
    public Vector3[] Normals { get; set; } = new Vector3[0];
    public Vector2[] UVs { get; set; } = new Vector2[0];
}
