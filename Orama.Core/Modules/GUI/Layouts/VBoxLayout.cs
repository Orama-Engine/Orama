using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI.Layouts;

/// <summary>
/// Orders widgets vertically.
/// </summary>
public class VBoxLayout : Layout
{
    /// <inheritdoc/>
    public override void LayoutChildren()
    {
        if (Parent == null)
            return;

        float currentY = Parent.StyleNormal.Padding;


        foreach (var widget in Parent.Children)
        {
            float width = widget.SizeHint.X;
            if (widget.HorizontalSizePolicy == SizePolicy.Expand)
                width = Parent.Rect.Width - 2 * Parent.StyleNormal.Padding;

            float height = widget.SizeHint.Y;
            if (widget.VerticalSizePolicy == SizePolicy.Expand)
                height = (Parent.Rect.Height - 2 * Parent.StyleNormal.Padding - Spacing * (Parent.Children.Count - 1)) / Parent.Children.Count;

            widget.Rect = new Rect(Parent.StyleNormal.Padding, currentY, width, height);

            currentY += height + Spacing;
        }
    }
}
