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
}
