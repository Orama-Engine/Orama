using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI.Layouts;

/// <summary>
/// Orders widgets vertically.
/// </summary>
public class VBoxLayout : Layout
{
    /// <summary> Number of columns. </summary>
    public int Columns { get; set; } = 3;

    public override void LayoutChildren()
    {
        if (Parent == null)
            return;

        float currentY = 0;


        foreach (var widget in Parent.Children)
        {
            // Determine width
            float width = widget.SizeHint.X;
            if (widget.HorizontalSizePolicy == SizePolicy.Expand)
            {
                width = Parent.Rect.Width;
            }

            // Determine height
            float height = widget.SizeHint.Y;
            if (widget.VerticalSizePolicy == SizePolicy.Expand)
            {
                height = (Parent.Rect.Height - Spacing * (Parent.Children.Count - 1)) / Parent.Children.Count;  // Equally distribute space
            }


            widget.Rect = new Rect(0, currentY, width, height);

            currentY += height + Spacing;
        }
    }
}
