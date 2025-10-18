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
        if (string.IsNullOrEmpty(text))
            return;

        // Approximate width per character
        float charWidth = font.Size * 0.6f;
        float charHeight = font.Size;

        Vector2 cursor = position;

        foreach (char c in text)
        {

            Rect charRect = new(cursor.X, cursor.Y, charWidth, charHeight);
            DrawRect(ref charRect, color);

            cursor.X += charWidth;
        }
    }
}
