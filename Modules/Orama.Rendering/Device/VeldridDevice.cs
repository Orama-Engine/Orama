// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using NeoVeldrid;

using Orama.Rendering.Device;

using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;

namespace Orama.Rendering.Veldrid;

/// <summary>
/// Interface into low-level Veldrid rendering.
/// </summary>
public class VeldridDevice
{
	/// <summary> The underlying Veldrid <see cref="global::Veldrid.GraphicsDevice"/>. </summary>
	public GraphicsDevice GraphicsDevice { get; private set; } = null!;

	/// <summary> The current frame number. </summary>
	public ulong CurrentFrame { get; internal set; }

	private RendererBackend backend;

	/// <summary> Initializes a new instance of the <see cref="VeldridDevice"/> class. </summary>
	public VeldridDevice(RendererBackend backend)
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

		switch (backend)
		{
			case RendererBackend.Vulkan:
			case RendererBackend.Direct3D11:
				{
					if (native is null)
						throw new NullReferenceException(nameof(native));

					SwapchainSource source = CreateSwapchainSource(native);

					SwapchainDescription desc = new(source, (uint)window.Size.X, (uint)window.Size.Y, null, window.VSync);
					GraphicsDevice = backend == RendererBackend.Vulkan ? GraphicsDevice.CreateVulkan(options, desc) : GraphicsDevice.CreateD3D11(options, desc);

					break;
				}
		}
	}

	public void SubmitCommands(CommandBuffer commandBuffer) => GraphicsDevice.SubmitCommands(commandBuffer.CommandList);

	/// <inheritdoc/>
	public void Resize(int width, int height) => GraphicsDevice.MainSwapchain.Resize((uint)width, (uint)height);

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
