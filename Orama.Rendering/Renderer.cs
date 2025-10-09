using Orama.Rendering.Backends;
using Silk.NET.Windowing;

namespace Orama.Rendering;

public enum RendererBackend
{
    OpenGL
}

public static class Renderer
{
    /// <summary> The renderer backend in use. </summary>
    public static RendererBackend Backend { get; private set; }

    private static readonly Dictionary<RendererBackend, IRendererBackend> backends = new()
    {
        { RendererBackend.OpenGL, new OpenGLBackend() }
    };

    /// <summary> Initializes the desired backend. Should be called once after window loading. </summary>
    /// <param name="window"> The window to initialize the backend for. </param>
    /// <param name="backend"> The backend to initialize. </param>
    public static void Initialize(IWindow window, RendererBackend backend)
    {
        Backend = backend;
        backends[backend].Initialize(window);
    }

    /// <summary> Renders the scene. </summary>
    public static void Render() => backends[Backend].Render();
}
