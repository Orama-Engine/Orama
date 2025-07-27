using System.Collections;

namespace Orama.Math;

/// <summary>
/// Represents a 2D integer vector.
/// </summary>
public struct Vector2I : IEquatable<Vector2I>, IReadOnlyList<int>, IFormattable, IComparable<Vector2I>, IComparable
{
	/// <summary>X component of the vector.</summary>
	public int X { get; set; }

	/// <summary>Y component of the vector.</summary>
	public int Y { get; set; }

	/// <summary>Zero vector.</summary>
	public static Vector2I Zero => new(0);

	/// <summary>Squared length of the vector.</summary>
	public long LengthSquared => (long)X * X + (long)Y * Y;

	/// <summary>
	/// Initializes a new vector with the specified component values.
	/// </summary>
	public Vector2I(int x, int y) => (X, Y) = (x, y);

	/// <summary>
	/// Initializes a new vector where all components have the same value.
	/// </summary>
	public Vector2I(int value) => (X, Y) = (value, value);

	/// <inheritdoc/>
	public int Count => 2;

	/// <inheritdoc/>
	public int this[int index] => index switch
	{
		0 => X,
		1 => Y,
		_ => throw new IndexOutOfRangeException(nameof(index))
	};

	/// <inheritdoc/>
	public int CompareTo(Vector2I other)
	{
		var xCompare = X.CompareTo(other.X);
		if (xCompare != 0)
			return xCompare;

		return Y.CompareTo(other.Y);
	}

	/// <inheritdoc/>
	public int CompareTo(object? obj) =>
		obj is Vector2I other ? CompareTo(other) : -1;

	/// <inheritdoc/>
	public bool Equals(Vector2I other) =>
		X.Equals(other.X) && Y.Equals(other.Y);

	/// <inheritdoc/>
	public override bool Equals(object? obj) =>
		obj is Vector2I other && Equals(other);

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(X, Y);

	/// <inheritdoc/>
	public IEnumerator<int> GetEnumerator()
	{
		yield return X;
		yield return Y;
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	public override string ToString() => ToString(null, null);

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		$"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)})";

	#region Operators

	public static Vector2I operator +(Vector2I a, Vector2I b) => new(a.X + b.X, a.Y + b.Y);
	public static Vector2I operator -(Vector2I a, Vector2I b) => new(a.X - b.X, a.Y - b.Y);
	public static Vector2I operator *(Vector2I a, Vector2I b) => new(a.X * b.X, a.Y * b.Y);
	public static Vector2I operator /(Vector2I a, Vector2I b) => new(a.X / b.X, a.Y / b.Y);
	public static Vector2I operator %(Vector2I a, Vector2I b) => new(a.X % b.X, a.Y % b.Y);

	public static Vector2I operator *(Vector2I v, int scalar) => new(v.X * scalar, v.Y * scalar);
	public static Vector2I operator *(int scalar, Vector2I v) => v * scalar;
	public static Vector2I operator /(Vector2I v, int scalar) => new(v.X / scalar, v.Y / scalar);
	public static Vector2I operator %(Vector2I v, int scalar) => new(v.X % scalar, v.Y % scalar);

	public static Vector2I operator -(Vector2I v) => new(-v.X, -v.Y);

	public static bool operator ==(Vector2I left, Vector2I right) => left.Equals(right);
	public static bool operator !=(Vector2I left, Vector2I right) => !(left == right);

	#endregion

	#region Methods

	/// <summary>Returns a normalized vector as floating point values.</summary>
	public Vector2 Normalize()
	{
		float length = MathF.Sqrt(X * X + Y * Y);
		if (length == 0f) return new Vector2(0f, 0f);
		return new Vector2(X / length, Y / length);
	}

	/// <summary>Returns the dot product of this vector and another.</summary>
	public int Dot(Vector2I other) => X * other.X + Y * other.Y;

	#endregion
}
