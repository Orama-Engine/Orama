using Orama.Math;

namespace Orama.Rendering;

/// <summary>
/// Contains information about the rendering frame.
/// </summary>
public readonly struct RenderFrame
{
    public Matrix4x4 View {  get; init; }
    public Matrix4x4 Projection { get; init; }
}
