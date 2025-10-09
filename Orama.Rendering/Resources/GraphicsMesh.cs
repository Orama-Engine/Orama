using System.Numerics;

namespace Orama.Rendering.Resources;

public enum WindingOrder
{
    CounterClockwise,
    Clockwise
}

public enum PrimitiveType
{
    /// <summary> Every 3 indices forms a triangle. </summary>
    TriangleList,

    /// <summary> Each new vertex after the first 2 forms a triangle with the previous 2 vertices. </summary>
    TriangleStrip,

    /// <summary> One central vertex, every consecutive pair forms a triangle. </summary>
    TriangleFan
}

/// <summary>
/// Lower level mesh used for rendering.
/// </summary>
public class GraphicsMesh
{
    public Vector3[] Vertices { get; set; } = new Vector3[0];
    public uint[] Indices { get; set; } = new uint[0];
    public Vector3[] Normals { get; set; } = new Vector3[0];
    public Vector2[] TexCoords { get; set; } = new Vector2[0];

    public GraphicsShader Shader { get; set; } = new();

    /// <summary> Winding order of the mesh's triangles. </summary>
    public WindingOrder WindingOrder { get; set; } = WindingOrder.CounterClockwise;

    /// <summary> Primitive type of the mesh. </summary>
    public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.TriangleList;
}