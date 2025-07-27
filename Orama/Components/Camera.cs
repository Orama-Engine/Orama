using Orama.Math;

namespace Orama.Components;

public class Camera : Component
{
	public static Camera? Main { get; private set; }

	public float FieldOfViewDegrees { get; set; } = 60f;
	public float AspectRatio { get; set; } = 16f / 9f;
	public float NearPlane { get; set; } = 0.1f;
	public float FarPlane { get; set; } = 1000f;

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
			return Matrix4x4.Invert(TransformMatrix);
		}
	}

	public Matrix4x4 ProjectionMatrix
	{
		get
		{
			float fovRadians = FieldOfViewDegrees * (float)(System.Math.PI / 180.0);
			return Matrix4x4.CreatePerspectiveFieldOfView(fovRadians, AspectRatio, NearPlane, FarPlane);
		}
	}
}
