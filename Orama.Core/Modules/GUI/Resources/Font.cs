using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace Orama.Core.Modules.GUI.Resources;

/// <summary>
/// A Text Font.
/// </summary>
public class Font
{
    /// <summary> The name of the font. </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary> Map of each character to its rectangle in the atlas. </summary>
    public Dictionary<char, Rect> GlyphMap { get; } = new();

    /// <summary> The set of characters to include in the atlas. </summary>
    private const string DefaultCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:'\",.<>?/ ";

    /// <summary> The size of the font. </summary>
    public int Size { get; }

    /// <summary> The font's atlas. </summary>
    /// <remarks> An atlas is a texture that contains all the characters in the font. </remarks>
    public Texture? Atlas { get; set; }

    /// <summary> internal font. </summary>
    private SixLabors.Fonts.Font? font;

    /// <summary> Initializes a new instance of the <see cref="Font"/> class from a .ttf path. </summary>
    public Font(string path, int size)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Font file not found", path);

        Name = Path.GetFileName(path);
        Size = size;

        var collection = new FontCollection();
        var fontFamily = collection.Add(path);
        font = fontFamily.CreateFont(size);
    }

    /// <summary> The default font. </summary>
    public static Font Default = new("default.ttf", 16);

    /// <summary> Renders the font atlas. </summary>
    public void RenderAtlas(int padding = 2)
    {
        if (font is null)
            return;

        // First, measure all glyphs and calculate atlas size (simple horizontal layout)
        var glyphImages = new Dictionary<char, Image<Rgba32>>();
        int totalWidth = 0;
        int maxHeight = 0;

        foreach (char c in DefaultCharacters)
        {
            var glyphRect = TextMeasurer.MeasureSize(c.ToString(), new TextOptions(font));
            int glyphWidth = (int)System.Math.Ceiling(glyphRect.Width) + padding * 2;
            int glyphHeight = (int)System.Math.Ceiling(glyphRect.Height) + padding * 2;

            var img = new Image<Rgba32>(glyphWidth, glyphHeight);
            img.Mutate(ctx =>
            {
                ctx.Clear(SixLabors.ImageSharp.Color.Transparent);
                ctx.DrawText(c.ToString(), font, SixLabors.ImageSharp.Color.White, new PointF(padding, padding));
            });

            glyphImages[c] = img;
            totalWidth += glyphWidth;
            maxHeight = System.Math.Max(maxHeight, glyphHeight);
        }

        // Create the final atlas image
        var atlasImage = new Image<Rgba32>(totalWidth, maxHeight);
        int cursorX = 0;

        foreach (var kvp in glyphImages)
        {
            char c = kvp.Key;
            var img = kvp.Value;

            atlasImage.Mutate(ctx =>
            {
                ctx.DrawImage(img, new Point(cursorX, 0), 1f);
            });

            // Store the rectangle in the atlas
            GlyphMap[c] = new Rect(cursorX, 0, img.Width, img.Height);
            cursorX += img.Width;

            img.Dispose();
        }

        // Save atlas
        byte[] pixelData = new byte[atlasImage.Width * atlasImage.Height * 4]; // RGBA8
        atlasImage.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var rowSpan = accessor.GetRowSpan(y);
                for (int x = 0; x < accessor.Width; x++)
                {
                    var pixel = rowSpan[x];
                    int offset = (y * accessor.Width + x) * 4;
                    pixelData[offset + 0] = pixel.R;
                    pixelData[offset + 1] = pixel.G;
                    pixelData[offset + 2] = pixel.B;
                    pixelData[offset + 3] = pixel.A;
                }
            }
        });

        Atlas = new Texture(atlasImage.Width, atlasImage.Height, Orama.Rendering.Resources.TextureDataType.RGBA8, pixelData);

        atlasImage.Dispose();
    }
}
