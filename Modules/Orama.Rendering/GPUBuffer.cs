using Orama.Common.Utility;
using Orama.Math;
using Orama.Rendering.Device;
using System.Runtime.InteropServices;

namespace Orama.Rendering;

/// <summary>
/// Buffer that assembles data in a format readable by a <see cref="CommandBuffer"/>.
/// </summary>
public struct GPUBuffer
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

    public void AddInt(int value)
    {
        EnsureAlignment(4);
        EnsureCapacity(offset + 4);

        Span<byte> dest = data.AsSpan(offset, 4);
        BitConverter.TryWriteBytes(dest, value);

        offset += 4;
    }

    public void AddFloat(float value)
    {
        EnsureAlignment(4);
        EnsureCapacity(offset + 4);

        Span<byte> dest = data.AsSpan(offset, 4);
        BitConverter.TryWriteBytes(dest, value);

        offset += 4;
    }

    public void AddFloat2(float x, float y)
    {
        AddFloat(x);
        AddFloat(y);
    }

    public void AddFloat3(float x, float y, float z)
    {
        AddFloat(x);
        AddFloat(y);
        AddFloat(z);
    }

    public void AddFloat4(float x, float y, float z, float w)
    {
        AddFloat(x);
        AddFloat(y);
        AddFloat(z);
        AddFloat(w);
    }

    public void AddMatrix4x4(Matrix4x4 m)
    {
        AddFloat(m.M11); AddFloat(m.M12); AddFloat(m.M13); AddFloat(m.M14);
        AddFloat(m.M21); AddFloat(m.M22); AddFloat(m.M23); AddFloat(m.M24);
        AddFloat(m.M31); AddFloat(m.M32); AddFloat(m.M33); AddFloat(m.M34);
        AddFloat(m.M41); AddFloat(m.M42); AddFloat(m.M43); AddFloat(m.M44);
    }

    /// <summary> Adds raw bytes directly to the buffer (e.g. for structs via MemoryMarshal). </summary>
    public void AddBytes(ReadOnlySpan<byte> bytes)
    {
        EnsureCapacity(offset + bytes.Length);
        bytes.CopyTo(data.AsSpan(offset));
        offset += bytes.Length;
    }

    /// <summary> Adds a blittable struct as raw bytes, respecting its natural alignment. </summary>
    public void AddStruct<T>(T value) where T : unmanaged
    {
        int size = Marshal.SizeOf<T>();
        EnsureAlignment(size >= 16 ? 16 : (size >= 8 ? 8 : 4));

        ReadOnlySpan<T> span = MemoryMarshal.CreateReadOnlySpan(ref value, 1);
        AddBytes(MemoryMarshal.AsBytes(span));
    }

    public void Reset() => offset = 0;

    private void EnsureAlignment(int alignment)
    {
        int aligned = Align(offset, alignment);
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
