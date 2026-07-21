// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common.Utility;
using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;

using Veldrith;
using Vortice.Vulkan;

namespace Orama.Rendering.Device.Implementations;

/// <summary>
/// Interface into low-level Veldrith rendering.
/// </summary>
internal class VeldrithDevice : IGraphicsDevice
{
	/// <summary> The underlying Veldrith <see cref="global::Veldrith.GraphicsDevice"/>. </summary>
	public GraphicsDevice GraphicsDevice { get; private set; } = null!;

	/// <inheritdoc/>
	public ulong CurrentFrame { get; set; }

	/// <inheritdoc/>
	public bool IsClipSpaceYInverted => GraphicsDevice.IsClipSpaceYInverted;

	private readonly RendererBackend backend;

	/// <summary> Initializes a new instance of the <see cref="VeldrithDevice"/> class. </summary>
	public VeldrithDevice(RendererBackend backend)
	{
		this.backend = backend;
	}

	/// <inheritdoc/>
	public void Initialize(IWindow window)
	{
		var native = window.Native;

		var options = new GraphicsDeviceOptions()
		{
#if DEBUG
			Debug = true,
#endif
			SyncToVerticalBlank = window.VSync,
		};

		if (native is null)
			throw new NullReferenceException(nameof(native));

		SwapchainSource source = CreateSwapchainSource(native);

		SwapchainDescription desc = new(source, (uint)window.Size.X, (uint)window.Size.Y, PixelFormat.D32FloatS8UInt, window.VSync);

		switch (backend)
		{
			case RendererBackend.Vulkan:
				GraphicsDevice = GraphicsDevice.CreateVulkan(options, desc);
				break;
			case RendererBackend.Direct3D12:
				GraphicsDevice = GraphicsDevice.CreateD3D12(options, desc);
				break;
		}

		CheckDebugTools(backend);
	}


	/// <inheritdoc/>
	public void SubmitCommands(ICommandBuffer commandBuffer)
	{
		var buffer = (VeldrithCommandBuffer)commandBuffer;
		GraphicsDevice.SubmitCommands(buffer.CommandList);
	}

	/// <inheritdoc/>
	public void ResizeSwapchain(uint width, uint height) => GraphicsDevice.MainSwapchain.Resize(width, height);

	/// <inheritdoc/>
	public void SwapBuffers() => GraphicsDevice.SwapBuffers();

	/// <inheritdoc/>
	public void Dispose() => GraphicsDevice.Dispose();

	/// <inheritdoc/>
	public ICommandBuffer GetCommandBuffer() => new VeldrithCommandBuffer(this);

	/// <summary> Checks if the debug tools are available for the given <see cref="RendererBackend"/>. </summary>
	/// <returns> <see langword="true"/> if the debug tools are available; otherwise, <see langword="false"/>. </returns>
	public static unsafe bool CheckDebugTools(RendererBackend backend)
	{
		if (backend == RendererBackend.Vulkan)
		{
			bool hasValidation = false;
			const string targetLayer = "VK_LAYER_KHRONOS_validation";

			uint layerCount = 0;
			VkResult result = Vulkan.vkEnumerateInstanceLayerProperties(&layerCount, null);

			if (result == VkResult.Success && layerCount > 0)
			{
				VkLayerProperties* availableLayers = stackalloc VkLayerProperties[(int)layerCount];
				result = Vulkan.vkEnumerateInstanceLayerProperties(&layerCount, availableLayers);

				if (result == VkResult.Success)
				{
					for (int i = 0; i < layerCount; i++)
					{
						byte* namePtr = availableLayers[i].layerName;

						string currentLayerName = System.Text.Encoding.UTF8.GetString(namePtr, GetNullTerminatedLength(namePtr, 256));

						if (currentLayerName == targetLayer)
						{
							hasValidation = true;
							break;
						}
					}
				}
			}

			if (!hasValidation)
				OramaConsole.Warning("Rendering debugging is enabled but Vulkan validation layers are not currently available, debugging capabilities will be limited. (https://vulkan.lunarg.com/sdk/home)");

			return hasValidation;
		}

		// TODO: D3D12
		if (backend == RendererBackend.Direct3D12)
		{
			OramaConsole.Warning("Direct3D12 debug tools are not currently available.");
			return false;
		}

		return false;
	}

	private static unsafe int GetNullTerminatedLength(byte* ptr, int maxLength)
	{
		int length = 0;
		while (length < maxLength && ptr[length] != 0)
			length++;

		return length;
	}

	private static SwapchainSource CreateSwapchainSource(INativeWindow? native)
	{
		ArgumentNullException.ThrowIfNull(native);

		if (native.Win32 is { } win32)
			return SwapchainSource.CreateWin32(win32.Hwnd, win32.HInstance);

		if (native.X11 is { } x11)
			return SwapchainSource.CreateXlib(x11.Display, (nint)x11.Window);

		if (native.Wayland is { } wayland)
			return SwapchainSource.CreateWayland(wayland.Display, wayland.Surface);

		throw new PlatformNotSupportedException("No supported native window handle was available.");
	}
}
