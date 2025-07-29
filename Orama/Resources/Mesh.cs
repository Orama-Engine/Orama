using System.Numerics;
using Orama.Rendering;
using Orama.Rendering.Materials;

namespace Orama.Resources;

public class Mesh : IClientRenderable
{
	// Default Cube Mesh
	public static Mesh Default => new Mesh
	{
		Vertices = new Vector3[]
		{
			// Front face
			new(-0.5f, -0.5f,  0.5f),
			new( 0.5f, -0.5f,  0.5f),
			new( 0.5f,  0.5f,  0.5f),
			new(-0.5f,  0.5f,  0.5f),

			// Back face
			new(-0.5f, -0.5f, -0.5f),
			new( 0.5f, -0.5f, -0.5f),
			new( 0.5f,  0.5f, -0.5f),
			new(-0.5f,  0.5f, -0.5f),
		},
		Indices = new uint[]
		{
			// Front
			0, 1, 2, 0, 2, 3,
			// Right
			1, 5, 6, 1, 6, 2,
			// Back
			5, 4, 7, 5, 7, 6,
			// Left
			4, 0, 3, 4, 3, 7,
			// Top
			3, 2, 6, 3, 6, 7,
			// Bottom
			4, 5, 1, 4, 1, 0
		},
		Material = Material.Default
	};

	public Vector3[] Vertices { get; set; } = [];
	public uint[] Indices { get; set; } = [];
	public Material Material { get; set; } = Material.Default;
	public Matrix4x4 ModelMatrix { get; set; } = Matrix4x4.Identity;
}
