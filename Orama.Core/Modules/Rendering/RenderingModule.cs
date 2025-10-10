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

    public void RenderMesh(Mesh mesh)
    {
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
