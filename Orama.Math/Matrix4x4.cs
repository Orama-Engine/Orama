
namespace Orama.Math;

/// <summary>
/// Represents a four-dimensional matrix.
/// </summary>
public struct Matrix4x4
{
    #region Components
    public float M11 { get; set; }
    public float M12 { get; set; }
    public float M13 { get; set; }
    public float M14 { get; set; }

    public float M21 { get; set; }
    public float M22 { get; set; }
    public float M23 { get; set; }
    public float M24 { get; set; }

    public float M31 { get; set; }
    public float M32 { get; set; }
    public float M33 { get; set; }
    public float M34 { get; set; }

    public float M41 { get; set; }
    public float M42 { get; set; }
    public float M43 { get; set; }
    public float M44 { get; set; }
    #endregion

    /// <summary> Creates a new instance of <see cref="Matrix4x4"/> with the specified components. </summary>
    public Matrix4x4(
    float m11, float m12, float m13, float m14,
    float m21, float m22, float m23, float m24,
    float m31, float m32, float m33, float m34,
    float m41, float m42, float m43, float m44)
    {
        M11 = m11; M12 = m12; M13 = m13; M14 = m14;
        M21 = m21; M22 = m22; M23 = m23; M24 = m24;
        M31 = m31; M32 = m32; M33 = m33; M34 = m34;
        M41 = m41; M42 = m42; M43 = m43; M44 = m44;
    }

    /// <summary> An identity matrix, this is comparable to <see cref="Vector3.One"/> but for matrices. </summary>
    public static Matrix4x4 Identity => new(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

    /// <summary> Creates a translation matrix from a <see cref="Vector3"/>. </summary>
    public static Matrix4x4 CreateTranslation(Vector3 pos)
    {
        return new Matrix4x4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            pos.X, pos.Y, pos.Z, 1
        );
    }

    /// <summary> Creates a scaling matrix from a <see cref="Vector3"/>. </summary>
    public static Matrix4x4 CreateScale(Vector3 scale)
    {
        return new Matrix4x4(
            scale.X, 0, 0, 0,
            0, scale.Y, 0, 0,
            0, 0, scale.Z, 0,
            0, 0, 0, 1
        );
    }

    public static Matrix4x4 CreateRotationX(float radians)
    {
        float c = MathF.Cos(radians);
        float s = MathF.Sin(radians);

        return new Matrix4x4(
            1, 0, 0, 0,
            0, c, s, 0,
            0, -s, c, 0,
            0, 0, 0, 1
        );
    }

    public static Matrix4x4 CreateRotationY(float radians)
    {
        float c = MathF.Cos(radians);
        float s = MathF.Sin(radians);

        return new Matrix4x4(
            c, 0, -s, 0,
            0, 1, 0, 0,
            s, 0, c, 0,
            0, 0, 0, 1
        );
    }

    public static Matrix4x4 CreateRotationZ(float radians)
    {
        float c = MathF.Cos(radians);
        float s = MathF.Sin(radians);

        return new Matrix4x4(
            c, s, 0, 0,
            -s, c, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
    }

    /// <summary> Creates a rotation matrix from Euler angles (XYZ order). </summary>
    public static Matrix4x4 CreateFromEuler(Vector3 euler)
    {
        Matrix4x4 rx = CreateRotationX(euler.X);
        Matrix4x4 ry = CreateRotationY(euler.Y);
        Matrix4x4 rz = CreateRotationZ(euler.Z);
        return rz * ry * rx;
    }

    /// <summary> Creates a rotation matrix from a <see cref="Quaternion"/>. </summary>
    public static Matrix4x4 CreateFromQuaternion(Quaternion q)
    {
        float xx = q.X * q.X;
        float yy = q.Y * q.Y;
        float zz = q.Z * q.Z;
        float xy = q.X * q.Y;
        float xz = q.X * q.Z;
        float yz = q.Y * q.Z;
        float wx = q.W * q.X;
        float wy = q.W * q.Y;
        float wz = q.W * q.Z;

        return new Matrix4x4(
            1 - 2 * (yy + zz), 2 * (xy + wz), 2 * (xz - wy), 0,
            2 * (xy - wz), 1 - 2 * (xx + zz), 2 * (yz + wx), 0,
            2 * (xz + wy), 2 * (yz - wx), 1 - 2 * (xx + yy), 0,
            0, 0, 0, 1
        );
    }

    /// <summary> Creates a right-handed LookAt view matrix. </summary>
    public static Matrix4x4 CreateLookAt(Vector3 eye, Vector3 target, Vector3 up)
    {
        Vector3 zAxis = Vector3.Normalize(eye - target); // Forward
        Vector3 xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis)); // Right
        Vector3 yAxis = Vector3.Cross(zAxis, xAxis); // True up

        return new Matrix4x4(
            xAxis.X, yAxis.X, zAxis.X, 0,
            xAxis.Y, yAxis.Y, zAxis.Y, 0,
            xAxis.Z, yAxis.Z, zAxis.Z, 0,
            -Vector3.Dot(xAxis, eye),
            -Vector3.Dot(yAxis, eye),
            -Vector3.Dot(zAxis, eye),
            1
        );
    }

    public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
    {
        return new Matrix4x4(
            a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41,
            a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42,
            a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43,
            a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44,

            a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41,
            a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42,
            a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43,
            a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44,

            a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41,
            a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42,
            a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43,
            a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44,

            a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41,
            a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42,
            a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43,
            a.M41 * b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44
        );
    }
}
