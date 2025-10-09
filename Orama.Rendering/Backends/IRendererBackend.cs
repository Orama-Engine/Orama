
using Orama.Rendering.Resources;
using Silk.NET.Windowing;

namespace Orama.Rendering.Backends;

/// <summary>
/// Represents a graphics backend.
/// </summary>
internal interface IRendererBackend
{
    /// <summary> Initializes the backend. </summary>
    /// <param name="window"> The window to initialize the backend for. </param>
    void Initialize(IWindow window);

    /// <summary> Renders the scene. </summary>
    /// <param name="renderQueue"> The render queue to render. </param>
    void Render(Queue<GraphicsMesh> renderQueue);

    /// <summary> Cleans up the backend. </summary>
    void Dispose();
}
