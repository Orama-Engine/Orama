using System.Numerics;

namespace Orama.Entities;

public class Transform
{
	public Vector3 Position { get; set; }
	public Quaternion Rotation { get; set; }
	public Vector3 Scale { get; set; }

	public Matrix4x4 Matrix =>
		Matrix4x4.CreateScale(Scale) *
		Matrix4x4.CreateFromQuaternion(Rotation) *
		Matrix4x4.CreateTranslation(Position);

	public Transform(Vector3 position, Quaternion rotation, Vector3 scale) =>
		(Position, Rotation, Scale) = (position, rotation, scale);

	public Transform() =>
		(Position, Rotation, Scale) = (Vector3.Zero, Quaternion.Identity, Vector3.One);

	// Forward direction (local -Z)
	public Vector3 Forward => Vector3.Transform(new Vector3(0, 0, -1), Rotation);

	// Right direction (local +X)
	public Vector3 Right => Vector3.Transform(new Vector3(1, 0, 0), Rotation);

	// Up direction (local +Y)
	public Vector3 Up => Vector3.Transform(new Vector3(0, 1, 0), Rotation);
}