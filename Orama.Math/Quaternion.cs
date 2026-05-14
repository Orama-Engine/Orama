
namespace Orama.Math;

/// <summary>
/// Represents a four-dimensional rotation.
/// </summary>
public struct Quaternion : IEquatable<Quaternion>
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

    /// <summary> Creates a <see cref="Quaternion"/> representing a rotation around an axis by an angle in radians. </summary>
    public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
    {
        axis = axis.Normalize();
        float halfAngle = angle * 0.5f;
        float sin = MathF.Sin(halfAngle);
        float cos = MathF.Cos(halfAngle);

        return new Quaternion(
            axis.X * sin,
            axis.Y * sin,
            axis.Z * sin,
            cos
        );
    }

    /// <summary> Converts this <see cref="Quaternion"/> to Euler angles (pitch, yaw, roll) in radians. </summary>
    public Vector3 ToEulerAngles()
    {
        Vector3 angles = new();

        // Pitch (X-axis rotation)
        float sinr_cosp = 2f * (W * X + Y * Z);
        float cosr_cosp = 1f - 2f * (X * X + Y * Y);
        angles.X = MathF.Atan2(sinr_cosp, cosr_cosp);

        // Yaw (Y-axis rotation)
        float sinp = 2f * (W * Y - Z * X);
        if (MathF.Abs(sinp) >= 1f)
            angles.Y = MathF.CopySign(MathF.PI / 2f, sinp); // use 90 degrees if out of range
        else
            angles.Y = MathF.Asin(sinp);

        // Roll (Z-axis rotation)
        float siny_cosp = 2f * (W * Z + X * Y);
        float cosy_cosp = 1f - 2f * (Y * Y + Z * Z);
        angles.Z = MathF.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }

    /// <summary> Creates a <see cref="Quaternion"/> from Euler angles (pitch, yaw, roll) in radians. </summary>
    public static Quaternion FromEulerAngles(float x, float y, float z)
    {
        float cy = MathF.Cos(z * 0.5f);
        float sy = MathF.Sin(z * 0.5f);
        float cp = MathF.Cos(y * 0.5f);
        float sp = MathF.Sin(y * 0.5f);
        float cr = MathF.Cos(x * 0.5f);
        float sr = MathF.Sin(x * 0.5f);

        return new Quaternion(
            sr * cp * cy - cr * sp * sy,
            cr * sp * cy + sr * cp * sy,
            cr * cp * sy - sr * sp * cy,
            cr * cp * cy + sr * sp * sy
        );
    }

    /// <inheritdoc cref="FromEulerAngles(float, float, float)"/>
    public static Quaternion FromEulerAngles(Vector3 angles) => FromEulerAngles(angles.X, angles.Y, angles.Z);

    #region Operators

    /// <summary> Quaternion multiplication (combines rotations). </summary>
    public static Quaternion operator *(Quaternion a, Quaternion b)
    {
        return new Quaternion(
            a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
            a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
            a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
            a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
        );
    }

    /// <summary> Negates a quaternion. </summary>
    public static Quaternion operator -(Quaternion q) => new(-q.X, -q.Y, -q.Z, -q.W);

    /// <summary> Checks equality of two quaternions. </summary>
    public static bool operator ==(Quaternion a, Quaternion b) =>
        a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

    /// <summary> Checks inequality of two quaternions. </summary>
    public static bool operator !=(Quaternion a, Quaternion b) => !(a == b);

    #endregion

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Quaternion q && this == q;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

    /// <summary> Checks equality of two quaternions. </summary>
    public bool Equals(Quaternion other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

    /// <inheritdoc/>
    public override string ToString() => $"({X}, {Y}, {Z}, {W})";
}
