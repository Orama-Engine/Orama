using Orama.Rendering;
using Orama.Resources;

namespace Orama.Components;

public class MeshRenderer : Component
{
	/// <summary>
	/// The mesh to render.
	/// </summary>
	public Mesh Mesh { get; set; } = Mesh.Default;

	public override void Update() => Renderer.AddRenderable(Mesh);
}
