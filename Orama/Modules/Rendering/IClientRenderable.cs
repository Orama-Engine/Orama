using System.Numerics;
using Orama.Rendering.Materials;
using Orama.Resources;

namespace Orama.Modules.Rendering;

public interface IClientRenderable
{
	public Vector3[] Vertices { get; }
	public uint[] Indices { get; }
	public Material Material { get; }
	public Matrix4x4 ModelMatrix { get; }
}
