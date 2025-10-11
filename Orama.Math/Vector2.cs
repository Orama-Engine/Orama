namespace Orama.Math;

/// <summary>
/// Represents a two-dimensional point.
/// </summary>
public struct Vector2
{
    /// <summary> The X component of the <see cref="Vector2"/>. </summary>
    public float X { get; set; }

    /// <summary> The Y component of the <see cref="Vector2"/>. </summary>
    public float Y { get; set; }

    /// <summary> Creates a new instance of <see cref="Vector2"/> with the specified components. </summary>
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    #region Casts

    public static implicit operator System.Numerics.Vector2(Vector2 v) => new(v.X, v.Y);
    public static implicit operator Vector2(System.Numerics.Vector2 v) => new(v.X, v.Y);

    #endregion
}
