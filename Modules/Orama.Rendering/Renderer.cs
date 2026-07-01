using NeoVeldrid;
using Orama.Rendering.Device;
using Orama.Rendering.Resources;
using Orama.Rendering.Veldrid;
using Silk.NET.Windowing;

namespace Orama.Rendering;

public enum RendererBackend
{
    OpenGL,
    Vulkan,
    Direct3D11
}

public static class Renderer
{
    /// <summary> The options for the renderer. </summary>
    public static RendererOptions Options { get; private set; }

    /// <summary> The renderer backend in use. </summary>
    public static RendererBackend Backend { get; private set; }

    /// <summary> The lower-level <see cref="VeldridDevice"/>. </summary>
    public static VeldridDevice Veldrid { get; private set; } = null!;

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
                _ when GraphicsDevice.IsBackendSupported(GraphicsBackend.Direct3D11) => RendererBackend.Direct3D11,
                _ when GraphicsDevice.IsBackendSupported(GraphicsBackend.OpenGL) => RendererBackend.OpenGL,

                _ => throw new InvalidOperationException("No supported graphics backend found.")
            };
        }

        Backend = backend.Value;

        Veldrid = new VeldridDevice(Backend);
        Veldrid.Initialize(window);
    }

    /// <summary> Presents the current frame. </summary>
    public static void Present()
    {
        Veldrid.GraphicsDevice.SwapBuffers();
        Veldrid.CurrentFrame++;

        FrameDisposalQueue.DisposeResources(Veldrid.CurrentFrame);
    }

    /// <summary> Submits the given <see cref="CommandBuffer"/> to be ran. </summary>
    public static void SubmitCommandBuffer(CommandBuffer commandBuffer) => Veldrid.SubmitCommands(commandBuffer);

    /// <summary> Resizes the renderer. </summary>
    public static void Resize(int width, int height) => Veldrid.Resize(width, height);

    /// <summary> Cleans up the renderer. </summary>
    public static void Dispose()
    {
        FrameDisposalQueue.DisposeResources(ulong.MaxValue);
        Veldrid.GraphicsDevice.Dispose();
    }
}
