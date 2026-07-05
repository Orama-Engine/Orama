using Orama.Rendering.Components;
using Orama.Rendering.Pipelines;
using Orama.Rendering.Pipelines.Forward;
using Orama.Rendering;
using Orama.Common;

namespace Orama.Rendering;

/// <summary>
/// Module responsible for rendering meshes to the window.
/// </summary>
public class RenderingModule : BaseModule
{
    /// <summary> The renderable objects to render next frame. </summary>
    internal List<IClientRenderable> Renderables { get; set; } = new();

    /// <summary> The <see cref="RenderPipeline"/> in use. </summary>
    public RenderPipeline Pipeline { get; set; } = new ForwardRenderPipeline();

    /// <inheritdoc/>
    public override void Initialize()
    {
        Application.OnResize +=  (size) => OnResize((int)size.X, (int)size.Y);
        Application.OnRender += Render;

        RendererOptions options = new();
        options.Culling = CullingMode.None;

        Renderer.Initialize(Application.Window.InternalWindow, RendererBackend.Direct3D11, options);
        Application.Window.Title += $" - [{Renderer.Backend}]";
    }

    public void Render() 
    {
        RenderFrame frame = new RenderFrame()
        {
            Camera = Camera.Main ?? new Camera(),
        };

        Pipeline.Render(in frame);
        Renderables.Clear();

        Renderer.Present();
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        base.Dispose();

        Renderer.Dispose();

        Application.OnResize -= (size) => OnResize((int)size.X, (int)size.Y);
        Application.OnRender -= Render;
    }

    /// <summary> Renders a client renderable to the window during the next frame. </summary>
    /// <param name="renderable">The object to render.</param>
    public void QueueObject(IClientRenderable renderable) => Renderables.Add(renderable);

    public void OnResize(int width, int height) => Renderer.Resize(width, height);
}
