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
		using var buffer = CommandBufferPool.Shared.RentAuto();
		buffer.Object.Begin();

		buffer.Object.SetFrameBuffer(TargetBuffer);
		buffer.Object.SetConstantBuffer("Camera", frame.CameraBuffer.Data);

		Render(in frame, buffer.Object);

		buffer.Object.End();
		Renderer.SubmitCommandBuffer(buffer.Object);
	}

	/// <summary> Performs rendering operations. </summary>
	/// <remarks> This method is called by <see cref="RenderPass.Execute(in RenderFrame)"/>. </remarks>
	public abstract void Render(in RenderFrame frame, ICommandBuffer buffer);

	/// <summary> The target <see cref="Framebuffer"/> this pass is rendering to. </summary>
	protected virtual Framebuffer TargetBuffer => Renderer.Veldrith.GraphicsDevice.SwapchainFramebuffer;
}
