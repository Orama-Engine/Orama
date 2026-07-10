using NeoVeldrid;
using Orama.Rendering.Device;

namespace Orama.Rendering.Pipelines;

/// <summary>
/// Render instructions for a step in the rendering pipeline.
/// </summary>
public abstract class RenderPass
{
    /// <summary> Fully runs this <see cref="RenderPass"/>. </summary>
    public void Execute(in RenderFrame frame)
    {
        var buffer = CommandBufferPool.Rent();
        buffer.Begin();

        buffer.CommandList.SetFramebuffer(TargetBuffer);
        buffer.QueueGPUBuffer(frame.CameraBuffer, "Camera");

        Render(in frame, buffer);

        buffer.End();
        Renderer.SubmitCommandBuffer(buffer);
        CommandBufferPool.Return(buffer);
    }

    /// <summary> Performs rendering operations. </summary>
    /// <remarks> This method is called by <see cref="RenderPass.Execute(in RenderFrame)"/>. </remarks>
    public abstract void Render(in RenderFrame frame, CommandBuffer buffer);

    /// <summary> The target <see cref="Framebuffer"/> this pass is rendering to. </summary>
    protected virtual Framebuffer TargetBuffer => Renderer.Veldrid.GraphicsDevice.SwapchainFramebuffer;
}
