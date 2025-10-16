
namespace Orama.Rendering.Resources;

public enum TextureDataType
{
    RGBA8,
    RGB8,
    RGBA16F,
    RGB16F,
    RGBA32F,
    RGB32F,
    R8,
    R16F,
    R32F,
    Depth24Stencil8
}

/// <summary>
/// Lower level texture used for rendering.
/// </summary>
public class GraphicsTexture
{
    /// <summary> Pixel data. </summary>
    public byte[] Data { get; set; }

    /// <summary> Texture width. </summary>
    public uint Width { get; set; }

    /// <summary> Texture height. </summary>
    public uint Height { get; set; }

    /// <summary> Data type. </summary>
    public TextureDataType Type { get; set; }

    /// <summary> Createes a new instance of <see cref="GraphicsTexture"/>. </summary>
    public GraphicsTexture(byte[] data, uint width, uint height, TextureDataType dataType)
    {
        Data = data;
        Width = width;
        Height = height;
        Type = dataType;
    }

    /// <summary> Createes a new instance of <see cref="GraphicsTexture"/>. </summary>
    public GraphicsTexture(uint width, uint height, TextureDataType dataType)
    {
        Width = width;
        Height = height;
        Type = dataType;
        Data = null!; // render target has no initial data
    }
}
