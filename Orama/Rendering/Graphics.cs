
using System;
using Veldrid;
using Veldrid.StartupUtilities;
using Vulkan;

namespace Orama.Rendering;

public static class Graphics
{
	public static GraphicsDevice Device { get; internal set; }
	public static ResourceFactory Factory => Device.ResourceFactory;
	public static Framebuffer ScreenTarget => Device.SwapchainFramebuffer;

	public static void Initialize(bool VSync = true, GraphicsBackend preferredBackend = GraphicsBackend.Vulkan)
	{
		GraphicsDeviceOptions deviceOptions = new()
		{
			SyncToVerticalBlank = VSync,
			ResourceBindingModel = ResourceBindingModel.Default,
			HasMainSwapchain = true,
			SwapchainDepthFormat = PixelFormat.D24_UNorm_S8_UInt,
			SwapchainSrgbFormat = false,
		};

		Device = VeldridStartup.CreateGraphicsDevice(Window.InternalWindow, deviceOptions, preferredBackend);
	}

	public static void EndFrame()
	{
		if (Device.SwapchainFramebuffer.Width != Window.Width || Device.SwapchainFramebuffer.Height != Window.Height)
			Device.ResizeMainWindow((uint)Window.Width, (uint)Window.Height);

		Device.WaitForIdle();

		Device.SwapBuffers();
	}

	internal static void Dispose()
	{
		Device.Dispose();
	}
}
