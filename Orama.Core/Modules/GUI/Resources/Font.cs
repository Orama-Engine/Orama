using Orama.Core.Modules.Rendering.Resources;
using SixLabors.Fonts;
using SixLabors.Fonts.Unicode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
    public Texture? Atlas
    {
        get
        {
            if (field == null)
                RenderAtlas();

            return field;
        }
        private set => field = value;
    }

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
    public void RenderAtlas(int padding = 0)
    {
        if (font is null)
            return;

        var glyphImages = new Dictionary<char, Image<Rgba32>>();
        int totalWidth = 0;
        int maxHeight = 0;

        var metrics = font.FontMetrics.VerticalMetrics;

        // All these are stored in Font Units
        short descender = metrics.Descender;
        short ascender = metrics.Ascender;
        short lineGap = metrics.LineGap;

        int lineHeight = (int)System.Math.Ceiling((ascender - descender + lineGap) * font.Size / font.FontMetrics.UnitsPerEm) + padding * 2;

        foreach (char c in DefaultCharacters)
        {
            if (!font.TryGetGlyphs(new CodePoint(c), out var glyphs) || glyphs == null || glyphs.Count == 0)
                continue;

            var glyphMetrics = glyphs[0].GlyphMetrics;

            // Convert from font units to pixels
            float scale = font.Size / font.FontMetrics.UnitsPerEm;
            int glyphWidth = (int)System.Math.Ceiling(glyphMetrics.AdvanceWidth * scale) + padding;
            int glyphHeight = lineHeight;

            var img = new Image<Rgba32>(glyphWidth, glyphHeight);
            int baselineOffset = (int)System.Math.Ceiling(ascender * scale) + padding;
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

                ctx.DrawText(graphicsOptions, c.ToString(), font, SixLabors.ImageSharp.Color.White, new PointF(padding, padding));
            });

            glyphImages[c] = img;
            totalWidth += glyphWidth;
            maxHeight = lineHeight;
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

        float minX = float.MaxValue, maxX = 0;
        float minY = float.MaxValue, maxY = 0;
        float cursorX = 0;

        foreach (char c in text)
        {
            if (!GlyphMap.TryGetValue(c, out var glyphRect))
                continue;

            minX = System.Math.Min(minX, cursorX);
            maxX = System.Math.Max(maxX, cursorX + glyphRect.Width);
            minY = System.Math.Min(minY, 0);
            maxY = System.Math.Max(maxY, glyphRect.Height);

            cursorX += glyphRect.Width;
        }

        return new Orama.Math.Vector2(maxX - minX, maxY - minY);
    }
}
