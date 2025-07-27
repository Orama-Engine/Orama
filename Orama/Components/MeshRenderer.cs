using Orama.Math;
using Orama.Rendering;

namespace Orama.Components;

public class MeshRenderer : Component, IClientRenderable
{
	public void GetRenderingData(out IGeometryDrawData drawData, out Matrix4x4 model)
	{
		model = Matrix4x4.Identity;
		drawData = null;
	}

	public override void Update()
	{

	}
}
