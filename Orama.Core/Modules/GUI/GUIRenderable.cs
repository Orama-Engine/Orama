using Orama.Core.Common.Utility;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;


namespace Orama.Core.Modules.GUI;

/// <summary>
/// A Drawable/Renderable GUI Component.
/// </summary>
internal class GUIRenderable : IClientRenderable
{
    public Matrix4x4 Transform { get; }

    public Vector3[] Vertices { get; }

    public Vector3[] Normals { get; }

    public Vector2[] UVs { get; }

    public uint[] Indices { get; }

    public Material Material { get; } = GUIMaterials.Rect.Clone();

    /// <summary> Initializes a new instance of the <see cref="GUIRenderable"/> class from a Rect. </summary>
    public GUIRenderable(Rect rect)
    {
        // Create a transform matrix for the GUI element
        Transform = Matrix4x4.CreateScale(rect.Width, rect.Height, 1f) *
                    Matrix4x4.CreateTranslation(rect.X, rect.Y, 0f);

        // Quad vertices (unit quad 0..1)
        Vertices = new Vector3[]
        {
            new Vector3(0, 0, 0), // top-left
            new Vector3(1, 0, 0), // top-right
            new Vector3(1, 1, 0), // bottom-right
            new Vector3(0, 1, 0), // bottom-left
        };

        // Normals (facing +Z)
        Normals = new Vector3[]
        {
            Vector3.UnitZ,
            Vector3.UnitZ,
            Vector3.UnitZ,
            Vector3.UnitZ
        };

        // UVs
        UVs = new Vector2[]
        {
            new Vector2(0, 0), // top-left
            new Vector2(1, 0), // top-right
            new Vector2(1, 1), // bottom-right
            new Vector2(0, 1)  // bottom-left
        };

        // Indices for two triangles
        Indices = new uint[]
        {
            0, 1, 2,
            0, 2, 3
        };
    }
}
