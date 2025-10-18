using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Handles positioning and sizing of widgets.
/// </summary>
public static class LayoutEngine
{
    /// <summary> Lays out a collection of widgets. </summary>
    public static void LayoutWidgets(IEnumerable<Widget> widgets)
    {
        foreach (var widget in widgets.Concat(widgets.SelectMany(w => w.Children)))
            widget.Rect = new Rect(widget.Rect.X, widget.Rect.Y, widget.SizeHint.X, widget.SizeHint.Y);
    }
}
