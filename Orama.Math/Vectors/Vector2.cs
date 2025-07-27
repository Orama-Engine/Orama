using System;
using System.Collections;

namespace Orama.Math;

/// <summary>
/// Represents a 2D floating-point vector.
/// </summary>
public struct Vector2 : IEquatable<Vector2>, IReadOnlyList<float>, IFormattable, IComparable<Vector2>, IComparable
{
    /// <summary>X component of the vector.</summary>
    public float X { get; set; }

    /// <summary>Y component of the vector.</summary>
    public float Y { get; set; }

    /// <summary>Zero vector.</summary>
    public static Vector2 Zero => new(0);


    /// <summary>Squared length of the vector.</summary>
    public float LengthSquared => X * X + Y * Y;

    /// <summary>Length (magnitude) of the vector.</summary>
    public float Length => MathF.Sqrt(LengthSquared);

    /// <summary>
    /// Initializes a new vector with the specified component values.
    /// </summary>
    public Vector2(float x, float y) => (X, Y) = (x, y);

    /// <summary>
    /// Initializes a new vector where all components have the same value.
    /// </summary>
    public Vector2(float value) => (X, Y) = (value, value);

    /// <inheritdoc/>
    public int Count => 2;

    /// <inheritdoc/>
    public float this[int index] => index switch
    {
        0 => X,
        1 => Y,
        _ => throw new IndexOutOfRangeException(nameof(index))
    };

    /// <inheritdoc/>
    public int CompareTo(Vector2 other)
    {
        var xCompare = X.CompareTo(other.X);
        if (xCompare != 0)
            return xCompare;

        return Y.CompareTo(other.Y);
    }

    /// <inheritdoc/>
    public int CompareTo(object? obj) =>
        obj is Vector2 other ? CompareTo(other) : -1;

    /// <inheritdoc/>
    public bool Equals(Vector2 other) =>
        X.Equals(other.X) && Y.Equals(other.Y);

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is Vector2 other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <inheritdoc/>
    public IEnumerator<float> GetEnumerator()
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

    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);
    public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.X / b.X, a.Y / b.Y);

    public static Vector2 operator +(Vector2 a, float b) => new(a.X + b, a.Y + b);
    public static Vector2 operator -(Vector2 a, float b) => new(a.X - b, a.Y - b);
    public static Vector2 operator *(Vector2 a, float b) => new(a.X * b, a.Y * b);
    public static Vector2 operator /(Vector2 a, float b) => new(a.X / b, a.Y / b);

    public static Vector2 operator -(Vector2 v) => new(-v.X, -v.Y);

    public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);
    public static bool operator !=(Vector2 left, Vector2 right) => !(left == right);

    #endregion

    #region Methods

    /// <summary>Returns a normalized (unit length) version of this vector.</summary>
    public Vector2 Normalize()
    {
        float length = Length;
        if (length == 0) return new Vector2(0, 0);
        float invLength = 1.0f / length;
        return new Vector2(X * invLength, Y * invLength);
    }

    /// <summary>Returns the dot product of this vector and another.</summary>
    public float Dot(Vector2 other) => X * other.X + Y * other.Y;

    #endregion
}
