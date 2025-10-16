using Orama.Core.Common.Utility;
using Orama.Core.Modules.Rendering;
using Orama.Math;
using Orama.Rendering;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Low-Level GUI Drawer.
/// </summary>
public static class Drawer
{
    /// <summary> Draws a rectangle. </summary>
    public static void DrawRect(ref Rect rect, Vector4 color)
    {
        GUIDrawable drawable = new(rect);
        drawable.Material.SetParameter("Color", color);
        ModuleManager.GetModule<RenderingModule>()?.QueueObject(drawable);
    }
}
