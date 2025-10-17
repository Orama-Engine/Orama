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

    public void OnResize(int width, int height) => Renderer.Resize(width, height);
}
