

using Orama.Components;
using System.Numerics;

namespace Orama.Rendering;

/// <summary>
/// Provides Context for rendering a frame.
/// </summary>
public struct RenderContext
{
	public Camera RenderingCamera = null!;

	public RenderContext(Camera targetCamera)
	{
		RenderingCamera = targetCamera;
	}

	public Matrix4x4 ViewMatrix => RenderingCamera.ViewMatrix;
	public Matrix4x4 ProjectionMatrix => RenderingCamera.ProjectionMatrix;
	public Matrix4x4 ViewProjectionMatrix => ViewMatrix * ProjectionMatrix;
}
