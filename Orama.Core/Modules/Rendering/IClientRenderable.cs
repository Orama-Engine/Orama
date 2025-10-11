
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;

namespace Orama.Core.Modules.Rendering;

/// <summary>
/// A Renderable object.
/// </summary>
public interface IClientRenderable
{
    Matrix4x4 Transform { get; }
    Vector3[] Vertices { get; }
    Vector3[] Normals { get; }
    Vector2[] UVs { get; }
    uint[] Indices { get; }
    Material Material { get; }
}
