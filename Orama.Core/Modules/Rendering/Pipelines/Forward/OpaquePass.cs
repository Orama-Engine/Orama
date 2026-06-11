using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Device;

namespace Orama.Core.Modules.Rendering.Pipelines.Forward;

/// <summary>
/// Pass responsible for rendering all solid objects.
/// </summary>
public class OpaquePass : RenderPass
{
    /// <inheritdoc/>
    public override void Render(ref RenderFrame frame)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;
        var buffer = CommandBufferPool.Rent();

        buffer.Begin();

        buffer.CommandList.SetFramebuffer(gd.SwapchainFramebuffer);

        buffer.ClearColor(Color.Black);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Pass == "Opaque")
            {
                Matrix4x4 model = renderable.Transform;
                Matrix4x4 view = frame.Camera.ViewMatrix;
                Matrix4x4 projection = frame.Camera.ProjectionMatrix;

                GPUBuffer paramBuffer = new GPUBuffer();
                paramBuffer.AddMatrix4x4(model);
                paramBuffer.AddMatrix4x4(view);
                paramBuffer.AddMatrix4x4(projection);
                buffer.QueueGPUBuffer(paramBuffer, 0);

                buffer.DrawRenderable(renderable, model);
            }

        buffer.End();
        Renderer.SubmitCommandBuffer(buffer);
        CommandBufferPool.Return(buffer);
    }
}
