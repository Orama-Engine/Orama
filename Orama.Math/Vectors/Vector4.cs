using System;
using System.Collections;

namespace Orama.Math;

/// <summary>
/// Represents a 4D floating-point vector.
/// </summary>
public struct Vector4 : IEquatable<Vector4>, IReadOnlyList<float>, IFormattable, IComparable<Vector4>, IComparable
{
    /// <summary>X component of the vector.</summary>
    public float X { get; set; }

    /// <summary>Y component of the vector.</summary>
    public float Y { get; set; }

    /// <summary>Z component of the vector.</summary>
    public float Z { get; set; }

    /// <summary>W component of the vector.</summary>
    public float W { get; set; }

    /// <summary>Zero vector.</summary>
    public static Vector4 Zero => new(0);

    /// <summary>Squared length of the vector.</summary>
    public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

    /// <summary>Length (magnitude) of the vector.</summary>
    public float Length => MathF.Sqrt(LengthSquared);

    /// <summary>
    /// Initializes a new vector with the specified component values.
    /// </summary>
    public Vector4(float x, float y, float z, float w) => (X, Y, Z, W) = (x, y, z, w);

    /// <summary>
    /// Initializes a new vector where all components have the same value.
    /// </summary>
    public Vector4(float value) => (X, Y, Z, W) = (value, value, value, value);

    /// <inheritdoc/>
    public int Count => 4;

    /// <inheritdoc/>
    public float this[int index] => index switch
    {
        0 => X,
        1 => Y,
        2 => Z,
        3 => W,
        _ => throw new IndexOutOfRangeException(nameof(index))
    };

    /// <inheritdoc/>
    public int CompareTo(Vector4 other)
    {
        int x = X.CompareTo(other.X);
        if (x != 0) return x;

        int y = Y.CompareTo(other.Y);
        if (y != 0) return y;

        int z = Z.CompareTo(other.Z);
        if (z != 0) return z;

        return W.CompareTo(other.W);
    }

    /// <inheritdoc/>
    public int CompareTo(object? obj) =>
        obj is Vector4 other ? CompareTo(other) : -1;

    /// <inheritdoc/>
    public bool Equals(Vector4 other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is Vector4 other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

    /// <inheritdoc/>
    public IEnumerator<float> GetEnumerator()
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

    public static Vector4 operator +(Vector4 a, Vector4 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    public static Vector4 operator -(Vector4 a, Vector4 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    public static Vector4 operator *(Vector4 a, Vector4 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
    public static Vector4 operator /(Vector4 a, Vector4 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

    public static Vector4 operator +(Vector4 a, float b) => new(a.X + b, a.Y + b, a.Z + b, a.W + b);
    public static Vector4 operator -(Vector4 a, float b) => new(a.X - b, a.Y - b, a.Z - b, a.W - b);
    public static Vector4 operator *(Vector4 a, float b) => new(a.X * b, a.Y * b, a.Z * b, a.W * b);
    public static Vector4 operator /(Vector4 a, float b) => new(a.X / b, a.Y / b, a.Z / b, a.W / b);

    public static Vector4 operator -(Vector4 v) => new(-v.X, -v.Y, -v.Z, -v.W);

    public static bool operator ==(Vector4 left, Vector4 right) => left.Equals(right);
    public static bool operator !=(Vector4 left, Vector4 right) => !(left == right);

    #endregion

    #region Methods

    /// <summary>Returns a normalized (unit length) version of this vector.</summary>
    public Vector4 Normalize()
    {
        float length = Length;
        if (length == 0) return new Vector4(0, 0, 0, 0);
        float invLength = 1.0f / length;
        return new Vector4(X * invLength, Y * invLength, Z * invLength, W * invLength);
    }

    /// <summary>Returns the dot product of this vector and another.</summary>
    public float Dot(Vector4 other) => X * other.X + Y * other.Y + Z * other.Z + W * other.W;

    #endregion
}
