using System;
using System.Collections;

namespace Orama.Math;

/// <summary>
/// Represents a 3D integer vector.
/// </summary>
public struct Vector3I : IEquatable<Vector3I>, IReadOnlyList<int>, IFormattable, IComparable<Vector3I>, IComparable
{
    /// <summary>X component of the vector.</summary>
    public int X { get; set; }

    /// <summary>Y component of the vector.</summary>
    public int Y { get; set; }

    /// <summary>Z component of the vector.</summary>
    public int Z { get; set; }

    /// <summary>Zero vector.</summary>
    public static Vector3I Zero => new(0);

    /// <summary>Squared length of the vector.</summary>
    public long LengthSquared =>
        (long)X * X + (long)Y * Y + (long)Z * Z;

    /// <summary>
    /// Initializes a new vector with the specified component values.
    /// </summary>
    public Vector3I(int x, int y, int z) => (X, Y, Z) = (x, y, z);

    /// <summary>
    /// Initializes a new vector where all components have the same value.
    /// </summary>
    public Vector3I(int value) => (X, Y, Z) = (value, value, value);

    /// <inheritdoc/>
    public int Count => 3;

    /// <inheritdoc/>
    public int this[int index] => index switch
    {
        0 => X,
        1 => Y,
        2 => Z,
        _ => throw new IndexOutOfRangeException(nameof(index))
    };

    /// <inheritdoc/>
    public int CompareTo(Vector3I other)
    {
        int xCompare = X.CompareTo(other.X);
        if (xCompare != 0)
            return xCompare;

        int yCompare = Y.CompareTo(other.Y);
        if (yCompare != 0)
            return yCompare;

        return Z.CompareTo(other.Z);
    }

    /// <inheritdoc/>
    public int CompareTo(object? obj) =>
        obj is Vector3I other ? CompareTo(other) : -1;

    /// <inheritdoc/>
    public bool Equals(Vector3I other) =>
        X == other.X && Y == other.Y && Z == other.Z;

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is Vector3I other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    /// <inheritdoc/>
    public IEnumerator<int> GetEnumerator()
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

    public static Vector3I operator +(Vector3I a, Vector3I b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3I operator -(Vector3I a, Vector3I b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3I operator *(Vector3I a, Vector3I b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    public static Vector3I operator /(Vector3I a, Vector3I b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
    public static Vector3I operator %(Vector3I a, Vector3I b) => new(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

    public static Vector3I operator *(Vector3I v, int scalar) => new(v.X * scalar, v.Y * scalar, v.Z * scalar);
    public static Vector3I operator *(int scalar, Vector3I v) => v * scalar;
    public static Vector3I operator /(Vector3I v, int scalar) => new(v.X / scalar, v.Y / scalar, v.Z / scalar);
    public static Vector3I operator %(Vector3I v, int scalar) => new(v.X % scalar, v.Y % scalar, v.Z % scalar);

    public static Vector3I operator -(Vector3I v) => new(-v.X, -v.Y, -v.Z);

    public static bool operator ==(Vector3I left, Vector3I right) => left.Equals(right);
    public static bool operator !=(Vector3I left, Vector3I right) => !(left == right);

    #endregion

    #region Methods

    /// <summary>Returns a normalized vector as floating point values.</summary>
    public Vector3 Normalize()
    {
        float length = MathF.Sqrt(X * X + Y * Y + Z * Z);
        if (length == 0f) return new Vector3(0f, 0f, 0f);
        return new Vector3(X / length, Y / length, Z / length);
    }

    /// <summary>Returns the dot product of this vector and another.</summary>
    public int Dot(Vector3I other) =>
        X * other.X + Y * other.Y + Z * other.Z;

    #endregion
}
