using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Device;
using Veldrid;

namespace Orama.Core.Modules.Rendering.Pipelines.Forward;

/// <summary> Pass responsible for rendering all solid objects. </summary>
public class OpaquePass : RenderPass
{
    public override void Render(ref RenderFrame frame)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;
        var buffer = Renderer.AllocateCommandBuffer();

        buffer.Begin();

        buffer.CommandList.SetFramebuffer(gd.SwapchainFramebuffer);

        buffer.ClearColor(Color.Black);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Pass == "Opaque")
                buffer.DrawRenderable(renderable);

        buffer.End();
        Renderer.SubmitCommandBuffer(buffer);
        buffer.Dispose();
    }
}
