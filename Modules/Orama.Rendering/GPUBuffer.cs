using Orama.Math;
using Orama.Rendering.Device;

namespace Orama.Rendering;

/// <summary>
/// Buffer that assembles data in a format readable by a <see cref="CommandBuffer"/>.
/// </summary>
public class GPUBuffer
{
    public IReadOnlyList<byte> Data => data;

    private List<byte> data = new();
    private int offset = 0;

    public void AddFloat(float value)
    {
        EnsureAlignment(4);

        data.AddRange(BitConverter.GetBytes(value));
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

    private void EnsureAlignment(int alignment)
    {
        int aligned = Align(offset, alignment);

        while (offset < aligned)
        {
            data.Add(0);
            offset++;
        }
    }

    protected static int Align(int offset, int alignment) => (offset + alignment - 1) / alignment * alignment;
}
