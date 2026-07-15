// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common.Utility;
using Orama.Math;
using Orama.Rendering.Device;
using Orama.Rendering.Resources;

namespace Orama.Rendering;

/// <summary>
/// Buffer that assembles data in a format readable by a <see cref="CommandBuffer"/>.
/// </summary>
public sealed class GPUBuffer
{
	/// <summary> The initial size of the internal buffer. </summary>
	public const int DEFAULT_SIZE = 256;

	/// <summary> Padded data of the buffer. </summary>
	public ReadOnlySpan<byte> Data
	{
		get
		{
			int paddedSize = Align(offset, 16);

			if (paddedSize > offset)
			{
				EnsureCapacity(paddedSize);
				data.AsSpan(offset, paddedSize - offset).Clear();
			}

			return data.AsSpan(0, paddedSize);
		}
	}

	private byte[] data = new byte[DEFAULT_SIZE];
	private int offset = 0;

	public GPUBuffer() { }

	/// <summary> Constructs a <see cref="GPUBuffer"/> from a <see cref="Material"/>s parameters. </summary>
	/// <remarks> <see cref="GPUBuffer"/>s created via this method should be returned to the pool via <c>GPUBufferPool.Instance.Return()</c> when no longer in use. </remarks>
	public static GPUBuffer ConstructFromMaterial(Material mat)
	{
		GPUBuffer buffer = GPUBufferPool.Instance.Rent();

		foreach (var param in mat.Shader.Parameters)
			switch (param)
			{
				case { Type: ShaderParameter.ParamType.Float, DefaultValue: float f }:
					buffer.AddFloat(f);
					break;

				case { Type: ShaderParameter.ParamType.Int, DefaultValue: long i }:
					buffer.AddInt((int)i);
					break;

				case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector3 v }:
					buffer.AddFloat3(v.X, v.Y, v.Z);
					break;

				case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector2 v }:
					buffer.AddFloat2(v.X, v.Y);
					break;

				case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector4 v }:
					buffer.AddFloat4(v.X, v.Y, v.Z, v.W);
					break;

				case { Type: ShaderParameter.ParamType.SampledTexture2D, DefaultValue: Texture text }:
					// TODO
					break;

				case { DefaultValue: null }:
					OramaConsole.Warning($"Parameter has no default value: {param.Name} ({param.Type})");
					break;

				default:
					OramaConsole.Warning($"Unsupported GPUBuffer solver for parameter: {param.Name} ({param.Type})");
					break;
			}

		return buffer;
	}

	public void AddInt(int value)
	{
		EnsurePacking(4, 4);
		EnsureCapacity(offset + 4);

		Span<byte> dest = data.AsSpan(offset, 4);
		BitConverter.TryWriteBytes(dest, value);

		offset += 4;
	}

	public void AddFloat(float value)
	{
		EnsurePacking(4, 4);
		EnsureCapacity(offset + 4);

		Span<byte> dest = data.AsSpan(offset, 4);
		BitConverter.TryWriteBytes(dest, value);

		offset += 4;
	}

	public void AddFloat2(float x, float y)
	{
		EnsurePacking(8, 8);
		EnsureCapacity(offset + 8);
		BitConverter.TryWriteBytes(data.AsSpan(offset, 4), x);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 4, 4), y);
		offset += 8;
	}

	public void AddFloat3(float x, float y, float z)
	{
		EnsurePacking(12, 16);
		EnsureCapacity(offset + 12);
		BitConverter.TryWriteBytes(data.AsSpan(offset, 4), x);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 4, 4), y);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 8, 4), z);
		offset += 12;
	}

	public void AddFloat4(float x, float y, float z, float w)
	{
		EnsurePacking(16, 16);
		EnsureCapacity(offset + 16);
		BitConverter.TryWriteBytes(data.AsSpan(offset, 4), x);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 4, 4), y);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 8, 4), z);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 12, 4), w);
		offset += 16;
	}

	public void AddMatrix4x4(Matrix4x4 m)
	{
		AddFloat4(m.M11, m.M12, m.M13, m.M14);
		AddFloat4(m.M21, m.M22, m.M23, m.M24);
		AddFloat4(m.M31, m.M32, m.M33, m.M34);
		AddFloat4(m.M41, m.M42, m.M43, m.M44);
	}

	/// <summary> Adds raw bytes directly to the buffer (e.g. for structs via MemoryMarshal). </summary>
	public void AddBytes(ReadOnlySpan<byte> bytes)
	{
		EnsureCapacity(offset + bytes.Length);
		bytes.CopyTo(data.AsSpan(offset));
		offset += bytes.Length;
	}

	public void Reset() => offset = 0;


	private void EnsurePacking(int size, int baseAlign)
	{
		int aligned = Align(offset, baseAlign);

		if ((aligned % 16) + size > 16)
			aligned = Align(aligned, 16);

		EnsureCapacity(aligned);
		while (offset < aligned)
		{
			data[offset] = 0;
			offset++;
		}
	}

	private void EnsureCapacity(int required)
	{
		if (required <= data.Length)
			return;

		int newSize = data.Length;
		while (newSize < required)
			newSize *= 2;

		Array.Resize(ref data, newSize);
	}

	private static int Align(int offset, int alignment) => (offset + alignment - 1) / alignment * alignment;
}
