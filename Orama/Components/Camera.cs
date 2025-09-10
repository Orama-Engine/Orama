using Orama.Echo;
using Orama.Rendering;
using System.Numerics;

namespace Orama.Components;

public class Camera : Component
{
	/// <summary>
	/// The main/currently active camera.
	/// </summary>
	public static Camera? Main { get; private set; }

	[Serialize] public float FieldOfViewDegrees { get; set; } = 60f;
	[Serialize] public float NearPlane { get; set; } = 0.1f;
	[Serialize] public float FarPlane { get; set; } = 1000f;

	public float AspectRatio { get; set; } = 16f / 9f;

	public Camera() => Main = this;

	// World Transform Matrix (Rotation + Position)
	public Matrix4x4 TransformMatrix
	{
		get
		{
			return Matrix4x4.CreateFromQuaternion(Transform.Rotation) * Matrix4x4.CreateTranslation(Transform.Position);
		}
	}

	// View Matrix
	public Matrix4x4 ViewMatrix
	{
		get
		{
			Matrix4x4.Invert(TransformMatrix, out var view);
			return view;
		}
	}

	// Projection Matrix
	public Matrix4x4 ProjectionMatrix
	{
		get
		{
			float aspectRatio = Window.Width / (float)Window.Height;
			float fovRadians = FieldOfViewDegrees * (float)(Math.PI / 180.0);
			var proj = Matrix4x4.CreatePerspectiveFieldOfView(fovRadians, aspectRatio, NearPlane, FarPlane);
			
			proj.M22 *= -1; // Manually flip the matrix's Y axis so it's not inverted, might create issues later on.

			return proj;
		}
	}
}
