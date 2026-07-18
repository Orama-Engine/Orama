// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device;

using Veldrith;

namespace Orama.Rendering.Pipelines;

/// <summary>
/// Render instructions for a step in the rendering pipeline.
/// </summary>
public abstract class RenderPass
{
	/// <summary> The <see cref="RenderPipeline"/> that owns this <see cref="RenderPass"/>. </summary>
	public RenderPipeline Pipeline { get; init; } = null!;

	/// <summary> Fully runs this <see cref="RenderPass"/>. </summary>
	public void Execute(in RenderFrame frame)
	{
		using var buffer = CommandBufferPool.Instance.RentAuto();
		buffer.Object.Begin();

		buffer.Object.CommandList.SetFramebuffer(TargetBuffer);
		buffer.Object.ResourceBinder.QueueGPUBuffer(frame.CameraBuffer, "Camera");

		Render(in frame, buffer);

		buffer.Object.DrawQueue();

		buffer.Object.End();
		Renderer.SubmitCommandBuffer(buffer);
	}

	/// <summary> Performs rendering operations. </summary>
	/// <remarks> This method is called by <see cref="RenderPass.Execute(in RenderFrame)"/>. </remarks>
	public abstract void Render(in RenderFrame frame, CommandBuffer buffer);

	/// <summary> The target <see cref="Framebuffer"/> this pass is rendering to. </summary>
	protected virtual Framebuffer TargetBuffer => Renderer.Veldrith.GraphicsDevice.SwapchainFramebuffer;
}
