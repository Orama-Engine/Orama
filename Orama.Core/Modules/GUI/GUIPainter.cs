using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Rendering;
using Orama.Rendering;
using Orama.Rendering.Resources;
using System.Numerics;

namespace Orama.Core.Modules.GUI;

public class GUIPainter
{
    /// <summary> The widget this painter is rendering. </summary>
    public BaseWidget Widget { get; set; }

    /// <summary> Initializes a new instance of the <see cref="GUIPainter"/> class. </summary>
    public GUIPainter(BaseWidget widget)
    {
        Widget = widget;
    }

    /// <summary> Draws a rectangle. </summary>
    public void DrawRect(Rect rect)
    {
        var vertices = new[]
        {
            new Vector3(rect.X, rect.Y, 0),
            new Vector3(rect.X + rect.Width, rect.Y, 0),
            new Vector3(rect.X + rect.Width, rect.Y + rect.Height, 0),
            new Vector3(rect.X, rect.Y + rect.Height, 0)
        };

        var indices = new uint[] { 0, 1, 2, 2, 3, 0 };

        var mesh = new GraphicsMesh
        {
            Vertices = vertices,
            Indices = indices,
            Shader = GUIShaders.Rect.GraphicsShader,
            Transform = Matrix4x4.Identity
        };

        Renderer.QueueMesh(mesh);
    }
}
