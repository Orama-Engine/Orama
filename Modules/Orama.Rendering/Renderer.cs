// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device;
using Orama.Rendering.Device.Implementations;
using Orama.Rendering.Resources;

using Silk.NET.Windowing;

using Veldrith;

namespace Orama.Rendering;

public enum RendererBackend
{
	Vulkan,
	Direct3D12
}

public static class Renderer
{
	/// <summary> The options for the renderer. </summary>
	public static RendererOptions Options { get; private set; }

	/// <summary> The renderer backend in use. </summary>
	public static RendererBackend Backend { get; private set; }

	/// <summary> The lower-level <see cref="IGraphicsDevice"/>. </summary>
	public static IGraphicsDevice Device { get; private set; } = null!;

	/// <summary> Initializes the desired backend. Should be called once after window loading. </summary>
	/// <param name="window"> The window to initialize the backend for. </param>
	/// <param name="backend"> The backend to initialize. </param>
	public static void Initialize(IWindow window, RendererBackend? backend = null, RendererOptions options = default)
	{
		Options = options;

		if (backend == null)
		{
			backend = true switch
			{
				_ when GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan) => RendererBackend.Vulkan,
				_ when GraphicsDevice.IsBackendSupported(GraphicsBackend.Direct3D12) => RendererBackend.Direct3D12,

				_ => throw new InvalidOperationException("No supported graphics backend found.")
			};
		}

		Backend = backend.Value;

		Device = new VeldrithDevice(Backend);
		Device.Initialize(window);
	}

	/// <summary> Presents the current frame. </summary>
	public static void Present()
	{
		Device.SwapBuffers();
		Device.CurrentFrame++;

		FrameDisposalQueue.DisposeResources(Device.CurrentFrame);
	}

	/// <summary> Submits the given <see cref="CommandBuffer"/> to be ran. </summary>
	public static void SubmitCommandBuffer(ICommandBuffer commandBuffer) => Device.SubmitCommands(commandBuffer);

	/// <summary> Resizes the renderer. </summary>
	public static void Resize(uint width, uint height) => Device.ResizeSwapchain(width, height);

	/// <summary> Cleans up the renderer. </summary>
	public static void Dispose()
	{
		FrameDisposalQueue.DisposeResources(ulong.MaxValue);
		Device.Dispose();
	}
}
