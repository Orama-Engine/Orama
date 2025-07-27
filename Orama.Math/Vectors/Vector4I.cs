using System;
using System.Collections;

namespace Orama.Math;

/// <summary>
/// Represents a 4D integer vector.
/// </summary>
public struct Vector4I : IEquatable<Vector4I>, IReadOnlyList<int>, IFormattable, IComparable<Vector4I>, IComparable
{
    /// <summary>X component of the vector.</summary>
    public int X { get; set; }

    /// <summary>Y component of the vector.</summary>
    public int Y { get; set; }

    /// <summary>Z component of the vector.</summary>
    public int Z { get; set; }

    /// <summary>W component of the vector.</summary>
    public int W { get; set; }

    /// <summary>Zero vector.</summary>
    public static Vector4I Zero => new(0);

    /// <summary>Squared length of the vector.</summary>
    public long LengthSquared =>
        (long)X * X + (long)Y * Y + (long)Z * Z + (long)W * W;

    /// <summary>
    /// Initializes a new vector with the specified component values.
    /// </summary>
    public Vector4I(int x, int y, int z, int w) => (X, Y, Z, W) = (x, y, z, w);

    /// <summary>
    /// Initializes a new vector where all components have the same value.
    /// </summary>
    public Vector4I(int value) => (X, Y, Z, W) = (value, value, value, value);

    /// <inheritdoc/>
    public int Count => 4;

    /// <inheritdoc/>
    public int this[int index] => index switch
    {
        0 => X,
        1 => Y,
        2 => Z,
        3 => W,
        _ => throw new IndexOutOfRangeException(nameof(index))
    };

    /// <inheritdoc/>
    public int CompareTo(Vector4I other)
    {
        int xCompare = X.CompareTo(other.X);
        if (xCompare != 0) return xCompare;

        int yCompare = Y.CompareTo(other.Y);
        if (yCompare != 0) return yCompare;

        int zCompare = Z.CompareTo(other.Z);
        if (zCompare != 0) return zCompare;

        return W.CompareTo(other.W);
    }

    /// <inheritdoc/>
    public int CompareTo(object? obj) =>
        obj is Vector4I other ? CompareTo(other) : -1;

    /// <inheritdoc/>
    public bool Equals(Vector4I other) =>
        X == other.X && Y == other.Y && Z == other.Z && W == other.W;

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is Vector4I other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

    /// <inheritdoc/>
    public IEnumerator<int> GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
        yield return W;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}, {W.ToString(format, formatProvider)})";

    #region Operators

    public static Vector4I operator +(Vector4I a, Vector4I b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    public static Vector4I operator -(Vector4I a, Vector4I b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    public static Vector4I operator *(Vector4I a, Vector4I b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
    public static Vector4I operator /(Vector4I a, Vector4I b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
    public static Vector4I operator %(Vector4I a, Vector4I b) => new(a.X % b.X, a.Y % b.Y, a.Z % b.Z, a.W % b.W);

    public static Vector4I operator *(Vector4I v, int scalar) => new(v.X * scalar, v.Y * scalar, v.Z * scalar, v.W * scalar);
    public static Vector4I operator *(int scalar, Vector4I v) => v * scalar;
    public static Vector4I operator /(Vector4I v, int scalar) => new(v.X / scalar, v.Y / scalar, v.Z / scalar, v.W / scalar);
    public static Vector4I operator %(Vector4I v, int scalar) => new(v.X % scalar, v.Y % scalar, v.Z % scalar, v.W % scalar);

    public static Vector4I operator -(Vector4I v) => new(-v.X, -v.Y, -v.Z, -v.W);

    public static bool operator ==(Vector4I left, Vector4I right) => left.Equals(right);
    public static bool operator !=(Vector4I left, Vector4I right) => !(left == right);

    #endregion

    #region Methods

    /// <summary>Returns a normalized vector as floating point values.</summary>
    public Vector4 Normalize()
    {
        float length = MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
        if (length == 0f) return new Vector4(0f, 0f, 0f, 0f);
        return new Vector4(X / length, Y / length, Z / length, W / length);
    }

    /// <summary>Returns the dot product of this vector and another.</summary>
    public int Dot(Vector4I other) =>
        X * other.X + Y * other.Y + Z * other.Z + W * other.W;

    #endregion
}
