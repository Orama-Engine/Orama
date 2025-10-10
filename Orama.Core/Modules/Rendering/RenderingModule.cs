using Orama.Core.Common;
using Orama.Core.Modules.Rendering.Resources;
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

    /// <summary> Renders a mesh to the window during the next frame. </summary>
    /// <param name="mesh">The mesh to render.</param>
    public void RenderMesh(Mesh mesh)
    {
        // TODO: Instantiating a new GraphicsMesh multiple times every frame is very expensive, don't do this
        GraphicsMesh graphicsMesh = new GraphicsMesh()
        {
            Vertices = mesh.Vertices,
            Indices = mesh.Indices,
            Normals = mesh.Normals,
            TexCoords = mesh.UVs,
            Shader = mesh.Material.Shader
        };

        Renderer.QueueMesh(graphicsMesh);
    }
}
