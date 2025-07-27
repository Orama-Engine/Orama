
using System.Collections;
using System.Numerics;

namespace Orama.Math.Vectors;

/// <summary>
/// Represents a 2D floating point vector.
/// </summary>
public struct Vector2<T> : IEquatable<Vector2<T>>, IReadOnlyList<T>, 
    IFormattable, IComparable
    where T : IFloatingPoint<T>
{
    // Index accessor
    public T this[int index] => index switch { 0 => X, 1 => Y, _ => throw new IndexOutOfRangeException() };

    /// <summary> The X component of the vector. </summary>
    public T X { get; set; }

    /// <summary> The Y component of the vector. </summary>
    public T Y { get; set; }

    /// <summary>Creates a vector with elements that have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X"/> parameter.</param>
    /// <param name="y">The value to assign to the <see cref="Y"/> parameter.</param>
    public Vector2(T x, T y) => (X, Y) = (x, y);

    /// <summary>Creates a vector with elements that have the specified value.</summary>
    /// <param name="value">The value to assign to the <see cref="X"/> and <see cref="Y"/> parameters.</param>
    public Vector2(T value) => (X, Y) = (value, value);

    public int Count => 2;

    public int CompareTo(object? obj)
    {
        if (obj is Vector2<T> vector)
            return CompareTo(vector);

        return -1;
    }

    public bool Equals(Vector2<T> other) => X.Equals(other.X) && Y.Equals(other.Y);

    public IEnumerator<T> GetEnumerator()
    {
        yield return X;
        yield return Y;
    }

    public string ToString(string? format, IFormatProvider? formatProvider) => $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)})";

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
