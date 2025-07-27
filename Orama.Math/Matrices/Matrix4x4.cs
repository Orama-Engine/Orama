using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Orama.Math;

/// <summary>
/// Represents a 4x4 floating-point matrix.
/// </summary>
public struct Matrix4x4 : IEquatable<Matrix4x4>, IFormattable, IReadOnlyList<float>
{
	public float M11, M12, M13, M14;
	public float M21, M22, M23, M24;
	public float M31, M32, M33, M34;
	public float M41, M42, M43, M44;

	/// <summary>Gets the identity matrix.</summary>
	public static Matrix4x4 Identity => new(
		1, 0, 0, 0,
		0, 1, 0, 0,
		0, 0, 1, 0,
		0, 0, 0, 1
	);

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

	public float this[int index] => index switch
	{
		0 => M11,
		1 => M12,
		2 => M13,
		3 => M14,
		4 => M21,
		5 => M22,
		6 => M23,
		7 => M24,
		8 => M31,
		9 => M32,
		10 => M33,
		11 => M34,
		12 => M41,
		13 => M42,
		14 => M43,
		15 => M44,
		_ => throw new IndexOutOfRangeException()
	};

	public int Count => 16;

	public Vector4 GetRow(int row) => row switch
	{
		0 => new(M11, M12, M13, M14),
		1 => new(M21, M22, M23, M24),
		2 => new(M31, M32, M33, M34),
		3 => new(M41, M42, M43, M44),
		_ => throw new ArgumentOutOfRangeException(nameof(row))
	};

	public Vector4 GetColumn(int column) => column switch
	{
		0 => new(M11, M21, M31, M41),
		1 => new(M12, M22, M32, M42),
		2 => new(M13, M23, M33, M43),
		3 => new(M14, M24, M34, M44),
		_ => throw new ArgumentOutOfRangeException(nameof(column))
	};

	public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
	{
		Matrix4x4 result = default;

		for (int row = 0; row < 4; row++)
		{
			for (int col = 0; col < 4; col++)
			{
				result[row, col] =
					a[row, 0] * b[0, col] +
					a[row, 1] * b[1, col] +
					a[row, 2] * b[2, col] +
					a[row, 3] * b[3, col];
			}
		}

		return result;
	}

	public static Vector4 operator *(Matrix4x4 m, Vector4 v) =>
		new(
			m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z + m.M14 * v.W,
			m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z + m.M24 * v.W,
			m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z + m.M34 * v.W,
			m.M41 * v.X + m.M42 * v.Y + m.M43 * v.Z + m.M44 * v.W
		);

	public float this[int row, int column]
	{
		get
		{
			if (row < 0 || row > 3 || column < 0 || column > 3)
				throw new IndexOutOfRangeException();
			return this[row * 4 + column];
		}
		set
		{
			switch (row * 4 + column)
			{
				case 0: M11 = value; break;
				case 1: M12 = value; break;
				case 2: M13 = value; break;
				case 3: M14 = value; break;
				case 4: M21 = value; break;
				case 5: M22 = value; break;
				case 6: M23 = value; break;
				case 7: M24 = value; break;
				case 8: M31 = value; break;
				case 9: M32 = value; break;
				case 10: M33 = value; break;
				case 11: M34 = value; break;
				case 12: M41 = value; break;
				case 13: M42 = value; break;
				case 14: M43 = value; break;
				case 15: M44 = value; break;
				default: throw new IndexOutOfRangeException();
			}
		}
	}

	public bool Equals(Matrix4x4 other) =>
		M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13) && M14.Equals(other.M14) &&
		M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23) && M24.Equals(other.M24) &&
		M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33) && M34.Equals(other.M34) &&
		M41.Equals(other.M41) && M42.Equals(other.M42) && M43.Equals(other.M43) && M44.Equals(other.M44);

	public override bool Equals(object? obj) =>
		obj is Matrix4x4 other && Equals(other);

	public override string ToString() => ToString(null, null);

	public string ToString(string? format, IFormatProvider? provider)
	{
		return string.Format(provider ?? CultureInfo.InvariantCulture,
			"{{ {0}, {1}, {2}, {3} | {4}, {5}, {6}, {7} | {8}, {9}, {10}, {11} | {12}, {13}, {14}, {15} }}",
			M11.ToString(format, provider), M12.ToString(format, provider), M13.ToString(format, provider), M14.ToString(format, provider),
			M21.ToString(format, provider), M22.ToString(format, provider), M23.ToString(format, provider), M24.ToString(format, provider),
			M31.ToString(format, provider), M32.ToString(format, provider), M33.ToString(format, provider), M34.ToString(format, provider),
			M41.ToString(format, provider), M42.ToString(format, provider), M43.ToString(format, provider), M44.ToString(format, provider)
		);
	}

	public IEnumerator<float> GetEnumerator()
	{
		yield return M11; yield return M12; yield return M13; yield return M14;
		yield return M21; yield return M22; yield return M23; yield return M24;
		yield return M31; yield return M32; yield return M33; yield return M34;
		yield return M41; yield return M42; yield return M43; yield return M44;
	}

	public override int GetHashCode() => (int)(M11 * M12) ^ (int)(M21 * M22) ^ (int)(M31 * M32);

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

	public static Matrix4x4 CreateTranslation(Vector3 position)
	{
		return new Matrix4x4(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			position.X, position.Y, position.Z, 1
		);
	}

	public static Matrix4x4 CreateTransform(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		// Scale * Rotation * Translation
		var scaleMatrix = new Matrix4x4(
			scale.X, 0, 0, 0,
			0, scale.Y, 0, 0,
			0, 0, scale.Z, 0,
			0, 0, 0, 1);

		var rotationMatrix = CreateFromQuaternion(rotation);
		var translationMatrix = CreateTranslation(position);

		return scaleMatrix * rotationMatrix * translationMatrix;
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public static bool operator ==(Matrix4x4 left, Matrix4x4 right) => left.Equals(right);
	public static bool operator !=(Matrix4x4 left, Matrix4x4 right) => !left.Equals(right);
}
