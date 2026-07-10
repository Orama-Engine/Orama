using Orama.Common;
using Orama.Math;
using Orama.Rendering.Device;

namespace Orama.Rendering.Pipelines.Forward;

/// <summary>
/// Pass responsible for rendering all solid objects.
/// </summary>
public class OpaquePass : RenderPass
{
    /// <inheritdoc/>
    public override void Render(in RenderFrame frame, CommandBuffer buffer)
    {
        buffer.ClearColor(Color.Black);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Shader.Pass == "Opaque")
                buffer.DrawRenderable(renderable);
    }
}
