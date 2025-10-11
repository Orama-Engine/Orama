
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

    /// <summary> Rotates a vector by this quaternion. </summary>
    public static Vector3 operator *(Quaternion q, Vector3 v)
    {
        // t = 2 * cross(q.xyz, v)
        Vector3 qVec = new Vector3(q.X, q.Y, q.Z);
        Vector3 t = 2 * Vector3.Cross(qVec, v);
        // result = v + w * t + cross(q.xyz, t)
        return v + q.W * t + Vector3.Cross(qVec, t);
    }
}
