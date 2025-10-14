using Orama.Core.Common;
using Orama.Core.Common.Components;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering;

/// <summary>
/// Module responsible for rendering meshes to the window.
/// </summary>
public class RenderingModule : BaseModule
{
    public override void Initialize()
    {
        Application.OnResize +=  (size) => OnResize((int)size.X, (int)size.Y);
        Application.OnRender += Render;

        Renderer.Initialize(Application.Window.InternalWindow, RendererBackend.OpenGL);
    }

    public void Render() 
    {
        Renderer.CommandBuffer.Clear(0f, 0f, 0f, 1f);
        Renderer.Render(Camera.Main != null ? (System.Numerics.Matrix4x4)Camera.Main.ViewMatrix : System.Numerics.Matrix4x4.Identity, Camera.Main != null ? (System.Numerics.Matrix4x4)Camera.Main.ProjectionMatrix : System.Numerics.Matrix4x4.Identity);
    }

    public override void Dispose() => Renderer.Dispose();

    /// <summary> Renders a client renderable to the window during the next frame. </summary>
    /// <param name="renderable">The object to render.</param>
    public void RenderObject(IClientRenderable renderable)
    {
        // TODO: Instantiating a new GraphicsMesh multiple times every frame is very expensive, don't do this
        var graphicsMesh = new GraphicsMesh()
        {
            Vertices = renderable.Vertices.Select(v => (System.Numerics.Vector3)v).ToArray(),
            Normals = renderable.Normals.Select(n => (System.Numerics.Vector3)n).ToArray(),
            TexCoords = renderable.UVs.Select(uv => (System.Numerics.Vector2)uv).ToArray(),
            Indices = renderable.Indices,
            Shader = renderable.Material.GraphicsShader,
            Transform = (System.Numerics.Matrix4x4)renderable.Transform
        };

        Renderer.QueueMesh(graphicsMesh);
    }

    public void OnResize(int width, int height) => Renderer.Resize(width, height);
}
