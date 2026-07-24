// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Math;

namespace Orama.Rendering;

/// <summary>
/// Contains information about the rendering frame.
/// </summary>
public readonly ref struct RenderFrame
{
	public Matrix4x4 View { get; init; }
	public Matrix4x4 Projection { get; init; }
	public ReadOnlySpan<byte> CameraBuffer { get; init; }
	public ReadOnlySpan<(IClientRenderable Renderable, Matrix4x4 Transform)> Renderables { get; init; }
}
