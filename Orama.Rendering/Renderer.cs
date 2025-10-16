using Orama.Rendering.Backends;
using Orama.Rendering.Backends.OpenGL;
using Orama.Rendering.Resources;
using Silk.NET.Windowing;
using System.Numerics;

namespace Orama.Rendering;

public enum RendererBackend
{
    OpenGL
}

public static class Renderer
{
    /// <summary> The options for the renderer. </summary>
    public static RendererOptions Options { get; private set; }

    /// <summary> The renderer backend in use. </summary>
    public static RendererBackend Backend { get; private set; }

    /// <summary> The command buffer for the current backend. </summary>
    public static ICommandBuffer CommandBuffer => backends[Backend].CommandBuffer;

    /// <summary> A First In First Out queue of meshes to render for the next frame. </summary>
    public static Queue<GraphicsMesh> RenderQueue { get; } = new();

    private static readonly Dictionary<RendererBackend, IRendererBackend> backends = new()
    {
        { RendererBackend.OpenGL, new OpenGLBackend() }
    };

    /// <summary> Initializes the desired backend. Should be called once after window loading. </summary>
    /// <param name="window"> The window to initialize the backend for. </param>
    /// <param name="backend"> The backend to initialize. </param>
    public static void Initialize(IWindow window, RendererBackend backend, RendererOptions options = default)
    {
        Options = options;
        Backend = backend;
        backends[backend].Initialize(window);
    }

    /// <summary> Renders the scene. </summary>
    public static void Render(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix) => backends[Backend].Render(RenderQueue, viewMatrix, projectionMatrix);

    /// <summary> Renders the scene offscreen to the provided <see cref="GraphicsTexture"/>. </summary>
    public static void RenderToTarget(GraphicsTexture renderTarget, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix) => backends[Backend].Render(RenderQueue, viewMatrix, projectionMatrix, renderTarget);

    /// <summary> Resizes the renderer. </summary>
    public static void Resize(int width, int height) => backends[Backend].Resize(width, height);

    /// <summary> Queues a mesh for rendering. Should be called once per frame for each desired mesh. </summary>
    /// <param name="mesh"> The mesh to queue. </param>
    public static void QueueMesh(GraphicsMesh mesh) => RenderQueue.Enqueue(mesh);

    /// <summary> Cleans up the renderer. </summary>
    public static void Dispose()
    {
        backends[Backend].Dispose();
    }
}
