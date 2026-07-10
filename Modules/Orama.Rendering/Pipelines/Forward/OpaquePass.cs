using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Device;
using Orama.Common;

namespace Orama.Rendering.Pipelines.Forward;

/// <summary>
/// Pass responsible for rendering all solid objects.
/// </summary>
public class OpaquePass : RenderPass
{
    /// <inheritdoc/>
    public override void Render(in RenderFrame frame)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;
        var buffer = CommandBufferPool.Rent();

        buffer.Begin();

        buffer.CommandList.SetFramebuffer(gd.SwapchainFramebuffer);

        buffer.ClearColor(Color.Black);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Shader.Pass == "Opaque")
            {
                Matrix4x4 model = renderable.Transform;
                Matrix4x4 view = frame.View;
                Matrix4x4 projection = frame.Projection;

                GPUBuffer globalBuffer = new GPUBuffer();
                globalBuffer.AddMatrix4x4(model);
                globalBuffer.AddMatrix4x4(view);
                globalBuffer.AddMatrix4x4(projection);
                buffer.QueueGPUBuffer(globalBuffer, 0);

                buffer.DrawRenderable(renderable);
            }

        buffer.End();
        Renderer.SubmitCommandBuffer(buffer);
        CommandBufferPool.Return(buffer);
    }
}
