using Orama.Core.Modules.Rendering.Resources;
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
    public string Name => font.Name;

    /// <summary> The family name of the font. </summary>
    public string FamilyName => font.Family.Name;

    /// <summary> The size of the font. </summary>
    public int Size { get; }

    /// <summary> Whether the font is smooth (anti-aliased) or not. </summary>
    public bool Smooth { get; set; } = true;

    /// <summary> The font's atlas. </summary>
    /// <remarks> An atlas is a texture that contains all the characters in the font. </remarks>
    public Texture? Atlas { get; set; }

    /// <summary> Map of each character to its rectangle in the atlas. </summary>
    public Dictionary<char, Rect> GlyphMap { get; } = new();

    // internal font.
    private SixLabors.Fonts.Font font;

    // The set of characters to include in the atlas.
    private const string DefaultCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:'\",.<>?/ ";

    /// <summary> Initializes a new instance of the <see cref="Font"/> class from a .ttf path. </summary>
    public Font(string path, int size)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Font file not found", path);

        var collection = new FontCollection();
        var fontFamily = collection.Add(path);
        font = fontFamily.CreateFont(size);

        Size = size;
    }

    /// <summary> Initializes a new instance of the <see cref="Font"/> class from an existing SixLabors.Fonts.Font. </summary>
    public Font(SixLabors.Fonts.Font existingFont, int size)
    {
        font = existingFont;
        Size = size;
    }

    /// <summary> The default system font. </summary>
    public static Font Default
    {
        get
        {
            if (field is not null)
                return field;

            // Try to pick a common font
            FontFamily? family = SystemFonts.Families.FirstOrDefault(f => f.Name == "Segoe UI" || f.Name == "Arial" || f.Name == "DejaVu Sans");

            // Fallback to first available font if none matched
            if (family is null)
                family = SystemFonts.Families.First();

            var systemFont = family?.CreateFont(16);

            field = new Font(systemFont ?? throw new Exception("Failed to create system font"), 16);
            return field;
        }
    }

    /// <summary> Renders the font atlas. </summary>
    public void RenderAtlas(int padding = 2)
    {
        if (font is null)
            return;

        var glyphImages = new Dictionary<char, Image<Rgba32>>();
        int totalWidth = 0;
        int maxHeight = 0;

        foreach (char c in DefaultCharacters)
        {
            var glyphRect = TextMeasurer.MeasureSize(c.ToString(), new TextOptions(font) { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left });
            int glyphWidth = (int)System.Math.Ceiling(glyphRect.Width) + padding * 2;
            int glyphHeight = (int)System.Math.Ceiling(glyphRect.Height) + padding * 2;
            

            var img = new Image<Rgba32>(glyphWidth, glyphHeight);
            float baselineOffset = padding;
            img.Mutate(ctx =>
            {
                ctx.Clear(SixLabors.ImageSharp.Color.Transparent);


                var graphicsOptions = new DrawingOptions
                {
                    GraphicsOptions = new GraphicsOptions
                    {
                        Antialias = Smooth
                    }
                };

                ctx.DrawText(graphicsOptions, c.ToString(), font, SixLabors.ImageSharp.Color.White, new PointF(padding, baselineOffset));
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

                    // Store alpha in red channel
                    pixelData[offset + 0] = pixel.A;
                    pixelData[offset + 1] = 0;
                    pixelData[offset + 2] = 0;
                    pixelData[offset + 3] = 255;
                }
            }
        });

        Atlas = new Texture(atlasImage.Width, atlasImage.Height, Orama.Rendering.Resources.TextureDataType.RGBA8, pixelData);

        atlasImage.Dispose();
    }

    /// <summary> Measures the size of the given text. </summary>
    public Orama.Math.Vector2 MeasureText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return new Orama.Math.Vector2(0, 0);

        var size = TextMeasurer.MeasureSize(text, new TextOptions(font)
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left
        });

        return new Orama.Math.Vector2(
            (float)size.Width + 15 * 2,
            (float)size.Height + 4 * 2
        );
    }
}
