namespace Orama.Math;

/// <summary>
/// Represents a three-dimensional point.
/// </summary>
public struct Vector3
{
    /// <summary> The X component of the <see cref="Vector3"/>. </summary>
    public float X { get; set; }

    /// <summary> The Y component of the <see cref="Vector3"/>. </summary>
    public float Y { get; set; }

    /// <summary> The Z component of the <see cref="Vector3"/>. </summary>
    public float Z { get; set; }

    /// <summary> Creates a new instance of <see cref="Vector3"/> with the specified components. </summary>
    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary> A Vector with all components set to zero. </summary>
    public static Vector3 Zero => new Vector3(0, 0, 0);

    /// <summary> A Vector with all components set to one. </summary>
    public static Vector3 One => new Vector3(1, 1, 1);

    /// <summary> A Vector set to the world's up direction. </summary>
    public static Vector3 Up => new Vector3(0, 1, 0);
}
