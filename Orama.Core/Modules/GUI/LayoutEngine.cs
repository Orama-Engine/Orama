using Orama.Core.Common;
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
        widgets.SelectMany(w => w.DescendantsAndSelf())
               .ToList()
               .ForEach(LayoutEngine.LayoutWidget);
    }

    /// <summary> Lays out a single widget. </summary>
    public static void LayoutWidget(Widget widget)
    {
        // Determine width
        float width = widget.SizeHint.X;
        switch (widget.HorizontalSizePolicy)
        {
            case SizePolicy.Fixed:
                width = widget.SizeHint.X;
                break;
            case SizePolicy.Expand:
                width = widget.Parent != null ? widget.Parent.Rect.Width : Application.Window.Size.X;
                break;
        }

        // Determine height
        float height = widget.SizeHint.Y;
        switch (widget.VerticalSizePolicy)
        {
            case SizePolicy.Fixed:
                height = widget.SizeHint.Y;
                break;
            case SizePolicy.Expand:
                height = widget.Parent != null ? widget.Parent.Rect.Height : Application.Window.Size.Y;
                break;
        }

        widget.Layout?.LayoutChildren();
        widget.Rect = new Rect(widget.Rect.X, widget.Rect.Y, width, height);
    }
}
