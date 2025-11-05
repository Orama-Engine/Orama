using Orama.Core.Modules.Rendering.Resources;
using Orama.Rendering.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Orama.Core.Common.Resources.DefaultProvider;

[ResourceLoader]
internal class TextureLoader : ResourceLoader<Texture>
{
    /// <inheritdoc/>
    public override Texture? LoadResource(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var image = Image.Load<Rgba32>(ms);

        int width = image.Width;
        int height = image.Height;

        byte[] pixelData = new byte[width * height * 4];
        image.CopyPixelDataTo(pixelData);

        return new Texture(width, height, TextureDataType.RGBA8, pixelData);
    }
}
