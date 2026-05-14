using Orama.Rendering.Device;
using Orama.Rendering.Resources;
using Orama.Rendering.Veldrid;
using Silk.NET.Windowing;
using System.Numerics;
using Veldrid;

namespace Orama.Rendering;

public enum RendererBackend
{
    OpenGL,
    Vulkan,
    DirectX11
}

public static class Renderer
{
    /// <summary> The options for the renderer. </summary>
    public static RendererOptions Options { get; private set; }

    /// <summary> The renderer backend in use. </summary>
    public static RendererBackend Backend { get; private set; }

    public static VeldridDevice Veldrid { get; private set; } = null!;

    /// <summary> Initializes the desired backend. Should be called once after window loading. </summary>
    /// <param name="window"> The window to initialize the backend for. </param>
    /// <param name="backend"> The backend to initialize. </param>
    public static void Initialize(IWindow window, RendererBackend backend, RendererOptions options = default)
    {
        Options = options;
        Backend = backend;

        Veldrid = new VeldridDevice(backend);
        Veldrid.Initialize(window);
    }

    /// <summary> Creates a new command buffer. </summary>
    public static CommandBuffer AllocateCommandBuffer() => new(Veldrid);

    public static void Present() => Veldrid.GraphicsDevice.SwapBuffers();

    public static void SubmitCommandBuffer(CommandBuffer commandBuffer) => Veldrid.SubmitCommands(commandBuffer);

    /// <summary> Resizes the renderer. </summary>
    public static void Resize(int width, int height) => Veldrid.Resize(width, height);

    /// <summary> Cleans up the renderer. </summary>
    public static void Dispose()
    {
        PipelineCache.Instance.Dispose();
    }
}
