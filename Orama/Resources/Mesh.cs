
using Orama.Math;
using Orama.Rendering;
using Veldrid;
using Vulkan;

namespace Orama.Resources;

public class Mesh : IGeometryDrawData
{
	public Vector3[] Vertices { get; set; }
	public int[] Indices { get; set; }

}
