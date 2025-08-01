

using Orama.Components;
using System.Numerics;

namespace Orama.Rendering;

/// <summary>
/// Provides Context for rendering a frame.
/// </summary>
public sealed class RenderContext
{
	public Camera RenderingCamera = null!;

	public Matrix4x4 ViewMatrix => RenderingCamera.ViewMatrix;
	public Matrix4x4 ProjectionMatrix => RenderingCamera.ProjectionMatrix;
	public Matrix4x4 ViewProjectionMatrix => ViewMatrix * ProjectionMatrix;
}
