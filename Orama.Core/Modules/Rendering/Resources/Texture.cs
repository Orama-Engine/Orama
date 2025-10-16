
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering.Resources;

/// <summary>
/// Represents a texture used in a material.
/// </summary>
public class Texture
{
    /// <summary> The underlying low-level texture. </summary>
    internal GraphicsTexture GraphicsTexture { get; private set; }

    /// <summary> Texture width in pixels. </summary>
    public int Width => (int)GraphicsTexture.Width;

    /// <summary> Texture height in pixels. </summary>
    public int Height => (int)GraphicsTexture.Height;

    /// <summary> Texture format. </summary>
    public TextureDataType Format => GraphicsTexture.Type;

    /// <summary>
    /// Creates a new <see cref="Texture"/> with optional pixel data.
    /// </summary>
    /// <param name="width">Texture width in pixels.</param>
    /// <param name="height">Texture height in pixels.</param>
    /// <param name="format">Pixel format.</param>
    /// <param name="initialData">Optional initial pixel data.</param>
    public Texture(int width, int height, TextureDataType format, byte[]? initialData = null)
    {
        GraphicsTexture = initialData != null
            ? new GraphicsTexture(initialData, (uint)width, (uint)height, format)
            : new GraphicsTexture((uint)width, (uint)height, format);
    }

    /// <summary>
    /// Wraps an existing <see cref="GraphicsTexture"/>.
    /// </summary>
    internal Texture(GraphicsTexture graphicsTexture)
    {
        GraphicsTexture = graphicsTexture ?? throw new ArgumentNullException(nameof(graphicsTexture));
    }

    /// <summary>
    /// Sets the raw pixel data.
    /// </summary>
    /// <param name="data">Byte array of pixel data matching texture format.</param>
    public void SetData(byte[] data)
    {
        if (data.Length != GraphicsTexture.Data.Length)
            throw new ArgumentException("Data length does not match texture size.");

        GraphicsTexture.Data = data;
    }

    /// <summary>
    /// Gets a copy of the raw pixel data.
    /// </summary>
    public byte[] GetData()
    {
        var copy = new byte[GraphicsTexture.Data.Length];
        Array.Copy(GraphicsTexture.Data, copy, GraphicsTexture.Data.Length);
        return copy;
    }

    /// <summary>
    /// Clears the texture to a specific color.
    /// </summary>
    public void Clear(byte r, byte g, byte b, byte a = 255)
    {
        int pixelSize = 4; // Assume RGBA8
        for (int i = 0; i < GraphicsTexture.Data.Length; i += pixelSize)
        {
            GraphicsTexture.Data[i + 0] = r;
            GraphicsTexture.Data[i + 1] = g;
            GraphicsTexture.Data[i + 2] = b;
            GraphicsTexture.Data[i + 3] = a;
        }
    }

    public static explicit operator Texture(GraphicsTexture graphicsTexture) => new Texture(graphicsTexture);
}
