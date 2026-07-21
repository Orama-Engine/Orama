// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Silk.NET.Windowing;

namespace Orama.Rendering.Device;

/// <summary>
/// Low-Level interface into the current Graphics API.
/// </summary>
public interface IGraphicsDevice : IDisposable
{
	/// <summary> The current frame. </summary>
	ulong CurrentFrame { get; set; }

	/// <summary> Whether the Y axis is inverted in clip space for this <see cref="IGraphicsDevice"/>. </summary>
	bool IsClipSpaceYInverted { get; }

	/// <summary> The <see cref="IFramebuffer"/> used by the main swapchain. </summary>
	IFramebuffer SwapchainFramebuffer { get; }

	/// <summary> Creates a new <see cref="ICommandBuffer"/>. </summary>
	ICommandBuffer GetCommandBuffer();

	/// <summary> Initializes the <see cref="IGraphicsDevice"/> for the given <see cref="IWindow"/>. </summary>
	void Initialize(IWindow window);

	/// <summary> Submits an <see cref="ICommandBuffer"/> for execution. </summary>
	void SubmitCommands(ICommandBuffer buffer);

	/// <summary> Swaps the front and back buffers. </summary>
	void SwapBuffers();

	/// <summary> Resizes the swapchain. </summary>
	void ResizeSwapchain(uint width, uint height);
}
