using Orama.Rendering;
using Orama.Rendering.Device;
using Veldrid;

namespace Orama.Core.Modules.Rendering.Pipelines.Forward;

/// <summary> Pass responsible for rendering all solid objects. </summary>
public class OpaquePass : RenderPass
{
    public override void Render()
    {
        var gd = Renderer.Veldrid.GraphicsDevice;
        var buffer = Renderer.CreateCommandBuffer();

        buffer.CommandList.Begin();
        buffer.CommandList.SetFramebuffer(gd.SwapchainFramebuffer);
        buffer.CommandList.ClearColorTarget(0, new RgbaFloat(0, 0, 0, 1));

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Pass == "Opaque")
                QueueObject(renderable, buffer);

        buffer.CommandList.End();
        Renderer.SubmitCommandBuffer(buffer);
    }
}
