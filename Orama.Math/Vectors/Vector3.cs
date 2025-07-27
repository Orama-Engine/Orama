using System.Collections;

namespace Orama.Math;

/// <summary>
/// Represents a 3D floating-point vector.
/// </summary>
public struct Vector3 : IEquatable<Vector3>, IReadOnlyList<float>, IFormattable, IComparable<Vector3>, IComparable
{
	/// <summary>X component of the vector.</summary>
	public float X { get; set; }

	/// <summary>Y component of the vector.</summary>
	public float Y { get; set; }

	/// <summary>Z component of the vector.</summary>
	public float Z { get; set; }

	/// <summary>Zero vector.</summary>
	public static Vector3 Zero => new(0);

	/// <summary>One vector.</summary>
	public static Vector3 One => new(1);

	/// <summary>Up vector.</summary>
	public static Vector3 Up => new(0, 1, 0);

	/// <summary>Squared length of the vector.</summary>
	public float LengthSquared => X * X + Y * Y + Z * Z;

	/// <summary>Length (magnitude) of the vector.</summary>
	public float Length => MathF.Sqrt(LengthSquared);

	/// <summary>
	/// Initializes a new vector with the specified component values.
	/// </summary>
	public Vector3(float x, float y, float z) => (X, Y, Z) = (x, y, z);

	/// <summary>
	/// Initializes a new vector where all components have the same value.
	/// </summary>
	public Vector3(float value) => (X, Y, Z) = (value, value, value);

	/// <inheritdoc/>
	public int Count => 3;

	/// <inheritdoc/>
	public float this[int index] => index switch
	{
		0 => X,
		1 => Y,
		2 => Z,
		_ => throw new IndexOutOfRangeException(nameof(index))
	};

	/// <inheritdoc/>
	public int CompareTo(Vector3 other)
	{
		int x = X.CompareTo(other.X);
		if (x != 0) return x;

		int y = Y.CompareTo(other.Y);
		if (y != 0) return y;

		return Z.CompareTo(other.Z);
	}

	/// <inheritdoc/>
	public int CompareTo(object? obj) =>
		obj is Vector3 other ? CompareTo(other) : -1;

	/// <inheritdoc/>
	public bool Equals(Vector3 other) =>
		X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

	/// <inheritdoc/>
	public override bool Equals(object? obj) =>
		obj is Vector3 other && Equals(other);

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(X, Y, Z);

	/// <inheritdoc/>
	public IEnumerator<float> GetEnumerator()
	{
		yield return X;
		yield return Y;
		yield return Z;
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	public override string ToString() => ToString(null, null);

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		$"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)})";

	#region Operators

	public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
	public static Vector3 operator /(Vector3 a, Vector3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

	public static Vector3 operator +(Vector3 a, float b) => new(a.X + b, a.Y + b, a.Z + b);
	public static Vector3 operator -(Vector3 a, float b) => new(a.X - b, a.Y - b, a.Z - b);
	public static Vector3 operator *(Vector3 a, float b) => new(a.X * b, a.Y * b, a.Z * b);
	public static Vector3 operator /(Vector3 a, float b) => new(a.X / b, a.Y / b, a.Z / b);

	public static Vector3 operator -(Vector3 v) => new(-v.X, -v.Y, -v.Z);

	public static bool operator ==(Vector3 left, Vector3 right) => left.Equals(right);
	public static bool operator !=(Vector3 left, Vector3 right) => !(left == right);

	#endregion

	#region Methods


	/// <summary>Returns a normalized (unit length) version of this vector.</summary>
	public Vector3 Normalize()
	{
		float length = Length;
		if (length == 0) return new Vector3(0, 0, 0);
		float invLength = 1.0f / length;
		return new Vector3(X * invLength, Y * invLength, Z * invLength);
	}

	/// <summary>Returns the dot product of this vector and another.</summary>
	public float Dot(Vector3 other) => X * other.X + Y * other.Y + Z * other.Z;

	/// <summary>Returns the cross product of this vector and another.</summary>
	public Vector3 Cross(Vector3 other) =>
		new(
			Y * other.Z - Z * other.Y,
			Z * other.X - X * other.Z,
			X * other.Y - Y * other.X
		);

	#endregion
}
