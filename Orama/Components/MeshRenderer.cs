using Orama.Echo;
using Orama.Rendering;
using Orama.Resources;
using System.Numerics;

namespace Orama.Components;

public class MeshRenderer : Component
{
	/// <summary>
	/// The mesh to render.
	/// </summary>
	[SerializeIgnore] public Mesh Mesh { get; set; } = Mesh.Default;

	public override void Start()
	{
		Mesh.Material.SetParameter<Vector4>("Color", new(1f, 1f, 1f, 1f));
	}

	public override void Update()
	{
		Mesh.ModelMatrix = Transform.Matrix;
		Renderer.AddRenderable(Mesh);
	}
}
