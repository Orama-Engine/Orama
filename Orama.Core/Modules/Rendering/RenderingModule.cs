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
    /// <summary> Optional override for view matrix. If null, Camera.Main.ViewMatrix is used. </summary>
    public Matrix4x4? ViewOverride { get; set; }

    /// <summary> Optional override for projection matrix. If null, Camera.Main.ProjectionMatrix is used. </summary>
    public Matrix4x4? ProjectionOverride { get; set; }

    public override void Initialize()
    {
        Application.OnResize +=  (size) => OnResize((int)size.X, (int)size.Y);
        Application.OnRender += Render;

        Renderer.Initialize(Application.Window.InternalWindow, RendererBackend.OpenGL);
    }

    public void Render()
    {
        // Determine which view matrix to use
        System.Numerics.Matrix4x4 view;
        if (ViewOverride.HasValue)
        {
            view = (System.Numerics.Matrix4x4)ViewOverride.Value;
        }
        else if (Camera.Main != null)
        {
            view = (System.Numerics.Matrix4x4)Camera.Main.ViewMatrix;
        }
        else
        {
            view = System.Numerics.Matrix4x4.Identity;
        }

        // Determine which projection matrix to use
        System.Numerics.Matrix4x4 proj;
        if (ProjectionOverride.HasValue)
        {
            proj = (System.Numerics.Matrix4x4)ProjectionOverride.Value;
        }
        else if (Camera.Main != null)
        {
            proj = (System.Numerics.Matrix4x4)Camera.Main.ProjectionMatrix;
        }
        else
        {
            proj = System.Numerics.Matrix4x4.Identity;
        }

        // Render with the chosen matrices
        Renderer.Render(view, proj);
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
