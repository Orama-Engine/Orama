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

    /// <summary> A First In First Out queue of <see cref="RenderItem"/>s to render for the next frame. </summary>
    public static Queue<RenderItem> RenderQueue { get; } = new();

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
    public static CommandBuffer CreateCommandBuffer() => new(Veldrid);

    public static void SubmitCommandBuffer(CommandBuffer commandBuffer) => Veldrid.GraphicsDevice.SubmitCommands(commandBuffer.CommandList);

    /// <summary> Renders all <see cref="RenderItem"/>s in the <see cref="RenderQueue"/>. </summary>
    public static void Render(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
    {
        Veldrid.Render(RenderQueue, viewMatrix, projectionMatrix);
        RenderQueue.Clear();
    }

    /// <summary> Renders all <see cref="RenderItem"/>s in the <see cref="RenderQueue"/> offscreen to the provided <see cref="Texture"/>. </summary>
    public static void RenderToTarget(Texture renderTarget, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
    {

        RenderQueue.Clear();
    }

    /// <summary> Resizes the renderer. </summary>
    public static void Resize(int width, int height) => Veldrid.Resize(width, height);

    /// <summary> Queues a <see cref="RenderItem"/> for rendering. Should be called once per frame for each desired item. </summary>
    /// <param name="item"> The item to queue. </param>
    public static void QueueMesh(RenderItem item) => RenderQueue.Enqueue(item);

    /// <summary> Cleans up the renderer. </summary>
    public static void Dispose()
    {
        PipelineCache.Instance.Dispose();
    }
}
