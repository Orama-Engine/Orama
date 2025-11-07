
using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering.Pipelines;

/// <summary>
/// Render instructions for a step in the rendering pipeline.
/// </summary>
public abstract class RenderPass
{
    public abstract void Render();

    /// <summary> Queues a renderable object to be rendered during the next frame. </summary>
    public void QueueObject(IClientRenderable renderable)
    {
        // TODO: Instantiating a new GraphicsMesh multiple times every frame is very expensive, don't do this
        var graphicsMesh = new GraphicsMesh()
        {
            Vertices = renderable.Vertices.Select(v => (System.Numerics.Vector3)v).ToArray(),
            Normals = renderable.Normals.Select(n => (System.Numerics.Vector3)n).ToArray(),
            TexCoords = renderable.UVs.Select(uv => (System.Numerics.Vector2)uv).ToArray(),
            Indices = renderable.Indices,
            Shader = renderable.Material.Shader.GraphicsShader,
            Transform = (System.Numerics.Matrix4x4)renderable.Transform
        };

        Renderer.QueueMesh(graphicsMesh);
    }
}
