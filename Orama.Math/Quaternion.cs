
namespace Orama.Math;

/// <summary>
/// Represents a four-dimensional rotation.
/// </summary>
public struct Quaternion
{
    /// <summary> The X component of the <see cref="Quaternion"/>. </summary>
    public float X { get; set; }

    /// <summary> The Y component of the <see cref="Quaternion"/>. </summary>
    public float Y { get; set; }

    /// <summary> The Z component of the <see cref="Quaternion"/>. </summary>
    public float Z { get; set; }

    /// <summary> The W component of the <see cref="Quaternion"/>. </summary>
    public float W { get; set; }

    /// <summary> Creates a new instance of <see cref="Quaternion"/> with the specified components. </summary>
    public Quaternion(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
}
