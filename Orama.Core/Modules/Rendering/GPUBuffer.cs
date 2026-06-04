using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;
using Orama.Rendering.Device;

namespace Orama.Core.Modules.Rendering;

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

    public void AddParameter(ShaderParameter p, object? value)
    {
        value ??= GetDefault(p.Type);

        // TODO: Dictionary
        switch (p.Type)
        {
            case ShaderParameter.ParamType.Float:
                AddFloat((float)value);
                break;

            case ShaderParameter.ParamType.Float2:
                var v2 = (Vector2)value;
                AddFloat2(v2.X, v2.Y);
                break;

            case ShaderParameter.ParamType.Float3:
                var v3 = (Vector3)value;
                AddFloat3(v3.X, v3.Y, v3.Z);
                break;

            case ShaderParameter.ParamType.Float4:
                var v4 = (Vector4)value;
                AddFloat4(v4.X, v4.Y, v4.Z, v4.W);
                break;

            case ShaderParameter.ParamType.Matrix:
                AddMatrix4x4((Matrix4x4)value);
                break;
        }
    }

    private object GetDefault(ShaderParameter.ParamType type)
    {
        return type switch
        {
            ShaderParameter.ParamType.Float => 0f,
            ShaderParameter.ParamType.Float2 => Vector2.Zero,
            ShaderParameter.ParamType.Float3 => Vector3.Zero,
            ShaderParameter.ParamType.Float4 => Vector4.Zero,
            ShaderParameter.ParamType.Matrix => Matrix4x4.Identity,
            _ => 0f
        };
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
