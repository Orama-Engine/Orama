// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device.Resources;
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

	/// <summary> The <see cref="IResourceFactory"/> used to create GPU resources. </summary>
	IResourceFactory ResourceFactory { get; }

	/// <summary> Initializes the <see cref="IGraphicsDevice"/> for <paramref name="window"/>. </summary>
	void Initialize(IWindow window);

	/// <summary> Submits <paramref name="buffer"/> for execution. </summary>
	void SubmitCommands(ICommandBuffer buffer);

	/// <summary> Swaps the front and back buffers. </summary>
	void SwapBuffers();

	/// <summary> Resizes the swapchain. </summary>
	void ResizeSwapchain(uint width, uint height);

	/// <summary> Updates <paramref name="buffer"/> with unmanaged <paramref name="data"/>. </summary>
	void UpdateBuffer<T>(IBuffer buffer, uint offset, ReadOnlySpan<T> data) where T : unmanaged;
}
