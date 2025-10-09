
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
    void Render();
}
