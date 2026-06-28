using Orama.Rendering.Components;

namespace Orama.Rendering;

/// <summary>
/// Contains information about the rendering frame.
/// </summary>
public readonly struct RenderFrame
{
    public Camera Camera { get; init; }
}
