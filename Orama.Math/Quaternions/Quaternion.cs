namespace Orama.Math.Vectors;

/// <summary>
/// Represents a quaternion for rotations.
/// </summary>
public struct Quaternion : IEquatable<Quaternion>, IFormattable
{
	/// <summary>X component.</summary>
	public float X { get; set; }

	/// <summary>Y component.</summary>
	public float Y { get; set; }

	/// <summary>Z component.</summary>
	public float Z { get; set; }

	/// <summary>W component.</summary>
	public float W { get; set; }

	/// <summary>
	/// Creates a new quaternion from components.
	/// </summary>
	public Quaternion(float x, float y, float z, float w) => (X, Y, Z, W) = (x, y, z, w);

	/// <summary>
	/// Creates a quaternion representing no rotation (identity).
	/// </summary>
	public static Quaternion Identity => new(0, 0, 0, 1);

	/// <summary>
	/// Returns the length squared of the quaternion.
	/// </summary>
	public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

	/// <summary>
	/// Returns the length (magnitude) of the quaternion.
	/// </summary>
	public float Length => MathF.Sqrt(LengthSquared);

	/// <summary>
	/// Returns the normalized quaternion.
	/// </summary>
	public Quaternion Normalized()
	{
		float length = Length;
		if (length == 0) return Identity;
		float invLength = 1.0f / length;
		return new Quaternion(X * invLength, Y * invLength, Z * invLength, W * invLength);
	}

	/// <summary>
	/// Returns the conjugate of the quaternion.
	/// </summary>
	public Quaternion Conjugate() => new Quaternion(-X, -Y, -Z, W);

	/// <summary>
	/// Returns the inverse of the quaternion.
	/// </summary>
	public Quaternion Inverse()
	{
		float lengthSq = LengthSquared;
		if (lengthSq == 0) return Identity;
		Quaternion conjugate = Conjugate();
		float invLengthSq = 1.0f / lengthSq;
		return new Quaternion(conjugate.X * invLengthSq, conjugate.Y * invLengthSq, conjugate.Z * invLengthSq, conjugate.W * invLengthSq);
	}

	/// <summary>
	/// Multiplies two quaternions.
	/// </summary>
	public static Quaternion operator *(Quaternion a, Quaternion b) =>
		new Quaternion(
			a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
			a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
			a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
			a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
		);

	/// <summary>
	/// Checks if two quaternions are equal.
	/// </summary>
	public bool Equals(Quaternion other) =>
		X == other.X && Y == other.Y && Z == other.Z && W == other.W;

	/// <inheritdoc/>
	public override bool Equals(object? obj) =>
		obj is Quaternion other && Equals(other);

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

	/// <inheritdoc/>
	public override string ToString() => ToString(null, null);

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		$"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}, {W.ToString(format, formatProvider)})";

	/// <summary>
	/// Creates a quaternion from an axis and angle in radians.
	/// </summary>
	public static Quaternion FromAxisAngle(Vector3 axis, float angleRadians)
	{
		float halfAngle = angleRadians * 0.5f;
		float sin = MathF.Sin(halfAngle);
		float cos = MathF.Cos(halfAngle);
		Vector3 normAxis = axis; // Should normalize axis here if not guaranteed normalized
		float length = MathF.Sqrt(normAxis.X * normAxis.X + normAxis.Y * normAxis.Y + normAxis.Z * normAxis.Z);
		if (length == 0) return Identity;
		float invLength = 1.0f / length;
		normAxis = new Vector3(normAxis.X * invLength, normAxis.Y * invLength, normAxis.Z * invLength);

		return new Quaternion(
			normAxis.X * sin,
			normAxis.Y * sin,
			normAxis.Z * sin,
			cos
		);
	}
}
