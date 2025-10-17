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
    public static void DrawRect(ref Rect rect, Vector4 color)
    {
        GUIRenderable drawable = new(rect);
        // drawable.Material.SetParameter("Color", (System.Numerics.Vector4)color);
        ModuleManager.GetModule<RenderingModule>()?.RenderObject(drawable);
    }
}
