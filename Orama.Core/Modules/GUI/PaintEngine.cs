using Orama.Core.Modules.GUI.Resources;
using Orama.Core.Modules.Rendering;
using Orama.Math;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Low-Level GUI Drawer.
/// </summary>
public static class PaintEngine
{
    /// <summary> Draws a rectangle. </summary>
    public static void DrawRect(ref Rect rect, Color color)
    {
        GUIRenderable drawable = new(rect);
        drawable.Material.SetParameter("u_Color", color);
        ModuleManager.GetModule<RenderingModule>()?.RenderObject(drawable);
    }

    /// <summary> Draws text. </summary>
    public static void DrawText(string text, Vector2 position, Color color, Font font)
    {
        if (font.Atlas == null || font.GlyphMap.Count == 0)
            font.RenderAtlas();

        if (string.IsNullOrEmpty(text) || font.Atlas == null || font.GlyphMap.Count == 0)
            return;

        Vector2 cursor = position;

        foreach (char c in text)
        {
            if (!font.GlyphMap.TryGetValue(c, out Rect glyphRect))
            {
                // Skip unknown characters or treat as space
                cursor.X += font.Size * 0.5f;
                continue;
            }

            // Create quad rect in screen space
            Rect charRect = new(cursor.X, cursor.Y, glyphRect.Width, glyphRect.Height);
            GUIRenderable charDrawable = new(charRect);
            charDrawable.Material = GUIMaterials.Text.Clone();

            charDrawable.Material.SetParameter("u_Color", color);
            charDrawable.Material.SetParameter("u_FontAtlas", font.Atlas);

            float atlasW = font.Atlas.Width;
            float atlasH = font.Atlas.Height;

            // Set UVs for this quad from glyph rectangle
            charDrawable.UVs = new Vector2[]
            {
                new Vector2(glyphRect.X / atlasW, glyphRect.Y / atlasH),                             // Top-left
                new Vector2((glyphRect.X + glyphRect.Width) / atlasW, glyphRect.Y / atlasH),       // Top-right
                new Vector2((glyphRect.X + glyphRect.Width) / atlasW, (glyphRect.Y + glyphRect.Height) / atlasH), // Bottom-right
                new Vector2(glyphRect.X / atlasW, (glyphRect.Y + glyphRect.Height) / atlasH)        // Bottom-left
            };

            ModuleManager.GetModule<RenderingModule>()?.RenderObject(charDrawable);

            cursor.X += glyphRect.Width; // advance cursor
        }
    }
}
