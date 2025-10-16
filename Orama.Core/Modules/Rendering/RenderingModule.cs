using Orama.Core.Common;
using Orama.Core.Modules.Rendering.Pipelines;
using Orama.Core.Modules.Rendering.Pipelines.Forward;
using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering;

/// <summary>
/// Module responsible for rendering meshes to the window.
/// </summary>
public class RenderingModule : BaseModule
{
    /// <summary> The renderable objects to render next frame. </summary>
    internal List<IClientRenderable> Renderables { get; set; } = new();

    /// <summary> The rendering pipeline in use. </summary>
    public RenderPipeline Pipeline { get; set; } = new ForwardRenderPipeline();

    public override void Initialize()
    {
        Application.OnResize +=  (size) => OnResize((int)size.X, (int)size.Y);
        Application.OnRender += Render;

        Renderer.Initialize(Application.Window.InternalWindow, RendererBackend.OpenGL);
    }

    public void Render() 
    {
        Pipeline.Render();
        Renderables.Clear();
    }

    public override void Dispose() => Renderer.Dispose();

    /// <summary> Renders a client renderable to the window during the next frame. </summary>
    /// <param name="renderable">The object to render.</param>
    public void RenderObject(IClientRenderable renderable) => Renderables.Add(renderable);

    /// <summary> Queues all renderable objects to be rendered during the next frame. </summary>
    public void QueueObject(IClientRenderable renderable)
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
