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

        Matrix4x4 view = frame.View;
        Matrix4x4 projection = frame.Projection;

        GPUBuffer paramBuffer = new GPUBuffer();
        paramBuffer.AddFloat(0.5f);
        buffer.QueueGPUBuffer(paramBuffer, 0, 0);

        GPUBuffer cameraBuffer = new GPUBuffer();
        cameraBuffer.AddMatrix4x4(view);
        cameraBuffer.AddMatrix4x4(projection);
        buffer.QueueGPUBuffer(cameraBuffer, 1, 0);

        buffer.ClearColor(Color.Black);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Shader.Pass == "Opaque")
            {
                Matrix4x4 model = renderable.Transform;

                GPUBuffer objectBuffer = new GPUBuffer();
                objectBuffer.AddMatrix4x4(model);
                buffer.QueueGPUBuffer(objectBuffer, 2, 0);

                buffer.DrawRenderable(renderable);
            }

        buffer.End();
        Renderer.SubmitCommandBuffer(buffer);
        CommandBufferPool.Return(buffer);
    }
}
