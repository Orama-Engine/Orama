// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.RHI.Resources;
using Silk.NET.Windowing;

namespace Orama.RHI.VulkanBackend;

/// <summary>
/// Interface into low-level Vulkan rendering.
/// </summary>
internal sealed class VulkanDevice : IGraphicsDevice
{
	/// <inheritdoc/>
	public VulkanInfo? VulkanInfo => throw new NotImplementedException();

	/// <inheritdoc/>
	public ulong CurrentFrame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	/// <inheritdoc/>
	public DeviceFeatures Features => throw new NotImplementedException();

	/// <inheritdoc/>
	public IFramebuffer SwapchainFramebuffer => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceFactory ResourceFactory => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Dispose() => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Initialize(IWindow window) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void ResizeSwapchain(uint width, uint height) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SubmitCommands(ICommandBuffer buffer) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SwapBuffers() => throw new NotImplementedException();

	/// <inheritdoc/>
	public void UpdateBuffer<T>(IBuffer buffer, uint offset, ReadOnlySpan<T> data) where T : unmanaged => throw new NotImplementedException();
}
