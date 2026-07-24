// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Math;
using Orama.Rendering.Resources;

namespace Orama.Rendering;

/// <summary>
/// A Renderable object.
/// </summary>
public interface IClientRenderable
{
	Vector3[] Vertices { get; }
	Vector3[] Normals { get; }
	Vector2[] UVs { get; }
	uint[] Indices { get; }
	Material Material { get; }
}
