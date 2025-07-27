using Orama.Rendering;
using Orama.Resources;

using Matrix4x4 = Orama.Math.Matrix4x4;
using Vector3 = Orama.Math.Vector3;

namespace Orama.Components;

internal class MeshRenderer : Component, IClientRenderable
{
	// Simple cube vertices centered at origin, length 1
	private static readonly Vector3[] _cubeVertices = new Vector3[]
	{
        // Front face
        new Vector3(-0.5f, -0.5f,  0.5f),
		new Vector3( 0.5f, -0.5f,  0.5f),
		new Vector3( 0.5f,  0.5f,  0.5f),
		new Vector3(-0.5f,  0.5f,  0.5f),

        // Back face
        new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3( 0.5f, -0.5f, -0.5f),
		new Vector3( 0.5f,  0.5f, -0.5f),
		new Vector3(-0.5f,  0.5f, -0.5f),
	};

	// Cube indices for 12 triangles (2 triangles per face * 6 faces)
	private static readonly uint[] _cubeIndices = new uint[]
	{
        // Front
        0, 1, 2,
		2, 3, 0,

        // Right
        1, 5, 6,
		6, 2, 1,

        // Back
        5, 4, 7,
		7, 6, 5,

        // Left
        4, 0, 3,
		3, 7, 4,

        // Top
        3, 2, 6,
		6, 7, 3,

        // Bottom
        4, 5, 1,
		1, 0, 4
	};

	public Matrix4x4 Transform => Matrix4x4.Identity;

	public Material Material => Material.Default;

	Math.Vector3[] IClientRenderable.Vertices => _cubeVertices;

	uint[] IClientRenderable.Indices => _cubeIndices;

	public override void Start()
	{
	}

	public override void Update()
	{
		Graphics.AddRenderable(this);
	}
}
