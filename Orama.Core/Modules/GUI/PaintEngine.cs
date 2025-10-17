using Orama.Core.Common.Utility;
using Orama.Core.Modules.Rendering;
using Orama.Math;
using Orama.Rendering;

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
}
