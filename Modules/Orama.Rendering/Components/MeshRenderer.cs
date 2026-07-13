// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Math;
using Orama.Rendering.Resources;
using Orama.Scenes.Components;

namespace Orama.Rendering.Components;

public class MeshRenderer : Component, IClientRenderable
{
	/// <summary> The <see cref="Resources.Mesh"/> to render. </summary>
	public Mesh? Mesh { get; set; }

	public Matrix4x4 Transform => Entity.Transform.Matrix;

	public Vector3[] Vertices => Mesh?.Vertices ?? Array.Empty<Vector3>();

	public Vector3[] Normals => Mesh?.Normals ?? Array.Empty<Vector3>();

	public Vector2[] UVs => Mesh?.UVs ?? Array.Empty<Vector2>();

	public uint[] Indices => Mesh?.Indices ?? Array.Empty<uint>();

	public Material Material => Mesh?.Material ?? Material.Default;

	/// <inheritdoc/>
	public override void Update()
	{
		if (Mesh == null)
			return;

		ModuleManager.GetModule<RenderingModule>()?.QueueObject(this);
	}
}
