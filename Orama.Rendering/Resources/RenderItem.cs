
using System.Numerics;

namespace Orama.Rendering.Resources;

/// <summary>
/// Low-Level representation of a rendered object.
/// </summary>
public readonly struct RenderItem
{
    public Matrix4x4 Transform { get; }
}
