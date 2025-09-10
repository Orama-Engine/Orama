using Orama.Echo;
using Orama.Rendering;
using Orama.Resources;

namespace Orama.Components;

public class MeshRenderer : Component
{
	/// <summary>
	/// The mesh to render.
	/// </summary>
	[SerializeIgnore] public Mesh Mesh { get; set; } = Mesh.Default;

	public override void Update()
	{
		Mesh.ModelMatrix = Transform.Matrix;
		Renderer.AddRenderable(Mesh);
	}
}
