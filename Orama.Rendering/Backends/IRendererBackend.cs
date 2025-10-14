
using Orama.Rendering.Resources;
using Silk.NET.Windowing;
using System.Numerics;

namespace Orama.Rendering.Backends;

/// <summary>
/// Represents a graphics backend.
/// </summary>
public interface IRendererBackend
{
    /// <summary> The command buffer for this backend. </summary>
    ICommandBuffer CommandBuffer { get; }

    /// <summary> Initializes the backend. </summary>
    /// <param name="window"> The window to initialize the backend for. </param>
    void Initialize(IWindow window);

    /// <summary> Renders the scene. </summary>
    /// <param name="renderQueue"> The render queue to render. </param>
    void Render(Queue<GraphicsMesh> renderQueue, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix);

    /// <summary> Resizes the renderer. </summary>
    /// <param name="width"> The new width. </param>
    /// <param name="height"> The new height. </param>
    void Resize(int width, int height);

    /// <summary> Cleans up the backend. </summary>
    void Dispose();
}
