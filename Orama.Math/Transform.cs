
namespace Orama.Math;

public class Transform
{
	public Vector3 Position { get; set; }
	public Quaternion Rotation { get; set; }
	public Vector3 Scale { get; set; }

	public Matrix4x4 Matrix => Matrix4x4.CreateTransform(Position, Rotation, Scale);

	public Transform(Vector3 position, Quaternion rotation, Vector3 scale) => (Position, Rotation, Scale) = (position, rotation, scale);

	public Transform() => (Position, Rotation, Scale) = (Vector3.Zero, Quaternion.Identity, Vector3.One);
}
