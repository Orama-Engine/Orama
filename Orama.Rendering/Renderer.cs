using Orama.Rendering.Device;
using Orama.Rendering.Resources;
using Orama.Rendering.Veldrid;
using Silk.NET.Windowing;
using System.Numerics;

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

    /// <summary> A First In First Out queue of meshes to render for the next frame. </summary>
    public static Queue<GraphicsMesh> RenderQueue { get; } = new();

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

    public static void SubmitCommandBuffer(CommandBuffer commandBuffer)
    {
        Veldrid.GraphicsDevice.SubmitCommands(commandBuffer.CommandList);
    }

    /// <summary> Renders all meshes in the <see cref="RenderQueue"/>. </summary>
    public static void Render(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
    {

        RenderQueue.Clear();
    }

    /// <summary> Renders all meshes in the <see cref="RenderQueue"/> offscreen to the provided <see cref="GraphicsTexture"/>. </summary>
    public static void RenderToTarget(GraphicsTexture renderTarget, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
    {

        RenderQueue.Clear();
    }

    /// <summary> Resizes the renderer. </summary>
    public static void Resize(int width, int height) => Veldrid.Resize(width, height);

    /// <summary> Queues a mesh for rendering. Should be called once per frame for each desired mesh. </summary>
    /// <param name="mesh"> The mesh to queue. </param>
    public static void QueueMesh(GraphicsMesh mesh) => RenderQueue.Enqueue(mesh);

    /// <summary> Cleans up the renderer. </summary>
    public static void Dispose() => Veldrid.Dispose();
}
