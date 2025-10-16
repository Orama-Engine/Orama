using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

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

    /// <summary> Creates a new <see cref="Texture"/> with optional pixel data. </summary>
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

    /// <summary> Wraps an existing <see cref="Orama.Rendering.Resources.GraphicsTexture"/>. </summary>
    internal Texture(GraphicsTexture graphicsTexture)
    {
        GraphicsTexture = graphicsTexture ?? throw new ArgumentNullException(nameof(graphicsTexture));
    }

    /// <summary> Sets the raw pixel data. </summary>
    /// <param name="data">Byte array of pixel data matching texture format.</param>
    public void SetData(byte[] data)
    {
        if (data.Length != GraphicsTexture.Data.Length)
            throw new ArgumentException("Data length does not match texture size.");

        GraphicsTexture.Data = data;
    }

    /// <summary> Gets a copy of the raw pixel data. </summary>
    public byte[] GetData()
    {
        var copy = new byte[GraphicsTexture.Data.Length];
        Array.Copy(GraphicsTexture.Data, copy, GraphicsTexture.Data.Length);
        return copy;
    }

    /// <summary> Clears the texture to a specific color. </summary>
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

    /// <summary> Saves the texture to a PNG file. </summary>
    /// <param name="path"></param>
    public void ToPng(string path)
    {
        if (GraphicsTexture.Type != TextureDataType.RGBA8 && GraphicsTexture.Type != TextureDataType.RGB8)
            throw new NotSupportedException("Only RGBA8 or RGB8 textures are supported.");

        using var image = new Image<Rgba32>(Width, Height);

        if (GraphicsTexture.Type == TextureDataType.RGBA8)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int i = (y * Width + x) * 4;
                    image[x, y] = new Rgba32(
                        GraphicsTexture.Data[i + 0], // R
                        GraphicsTexture.Data[i + 1], // G
                        GraphicsTexture.Data[i + 2], // B
                        GraphicsTexture.Data[i + 3]  // A
                    );
                }
            }
        }
        else if (GraphicsTexture.Type == TextureDataType.RGB8)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int i = (y * Width + x) * 3;
                    image[x, y] = new Rgba32(
                        GraphicsTexture.Data[i + 0], // R
                        GraphicsTexture.Data[i + 1], // G
                        GraphicsTexture.Data[i + 2], // B
                        255                           // full alpha
                    );
                }
            }
        }

        image.Save(path, new PngEncoder());
    }

    public static explicit operator Texture(GraphicsTexture graphicsTexture) => new Texture(graphicsTexture);
}
