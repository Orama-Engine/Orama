using Orama.Rendering.Device;

namespace Orama.Core.Modules.Rendering;

/// <summary>
/// Buffer that assembles data in a format readable by a <see cref="CommandBuffer"/>.
/// </summary>
public class GPUBuffer
{
    /// <summary> The underlying byte data. </summary>
    public IReadOnlyList<byte> Data
    {
        get
        {
            Pad16();
            return data;
        }
    }

    private List<byte> data = new();

    public void AddFloat(float value) => data.AddRange(BitConverter.GetBytes(value));
    public void AddFloat2(float x, float y) { AddFloat(x); AddFloat(y); }
    public void AddFloat3(float x, float y, float z) { AddFloat(x); AddFloat(y); AddFloat(z); }
    public void AddFloat4(float x, float y, float z, float w) { AddFloat(x); AddFloat(y); AddFloat(z); AddFloat(w); }

    // Pads the buffer to the next 16-byte boundary.
    private void Pad16() 
    {
        int remainder = data.Count % 16;
        if (remainder != 0)
        {
            int padding = 16 - remainder;
            for (int i = 0; i < padding; i++)
                data.Add(0);
        }
    }
}
