using Orama.Core.Common;
using Orama.Rendering;

namespace Orama.Core.Modules.Rendering;

/// <summary>
/// Module responsible for rendering meshes to the window.
/// </summary>
public class RenderingModule : BaseModule
{
    public override void Initialize()
    {
        Renderer.Initialize(Application.Window.InternalWindow, RendererBackend.OpenGL);
    }

    public override void Update()
    {
        // TODO: Expose Application.Render to modules?
        Renderer.Render();
    }

    public override void Dispose()
    {
        Renderer.Dispose();
    }
}
