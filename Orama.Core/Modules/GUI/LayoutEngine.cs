using Orama.Core.Common;
using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Handles positioning and sizing of widgets.
/// </summary>
public static class LayoutEngine
{
    /// <summary> Lays out a collection of widgets. </summary>
    public static void LayoutWidgets(IEnumerable<Widget> roots)
    {
        foreach (var root in roots)
            LayoutSubtree(root);
    }

    /// <summary> Layout a widget and all of its descendants. </summary>
    private static void LayoutSubtree(Widget widget)
    {
        ComputeSize(widget);

        widget.Layout?.LayoutChildren();

        foreach (var child in widget.Children)
            LayoutSubtree(child);
    }


    /// <summary> Compute absolute size of a widget. </summary>
    private static void ComputeSize(Widget w)
    {
        float parentWidth = w.Parent?.Rect.Width ?? Application.Window.Size.X;
        float parentHeight = w.Parent?.Rect.Height ?? Application.Window.Size.Y;

        // Horizontal
        float width = w.HorizontalSizePolicy switch
        {
            SizePolicy.Fixed => w.SizeHint.X,
            SizePolicy.Expand => parentWidth,
            _ => w.SizeHint.X
        };

        // Vertical
        float height = w.VerticalSizePolicy switch
        {
            SizePolicy.Fixed => w.SizeHint.Y,
            SizePolicy.Expand => parentHeight,
            _ => w.SizeHint.Y
        };

        w.Rect = new Rect(w.Rect.X, w.Rect.Y, width, height);
    }
}
