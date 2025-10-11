
namespace Orama.Core.Modules.GUI.Widgets;

/// <summary>
/// Base class for all GUI widgets.
/// </summary>
public class BaseWidget
{
    /// <summary> The painter of the widget. </summary>
    protected GUIPainter Painter { get; set; }

    /// <summary> The position and size of the widget. </summary>
    public Rect Rect { get; set; }

    /// <summary> The styling of the widget. </summary>
    public WidgetStyle Style { get; set; }

    /// <summary> Initializes a new instance of the <see cref="BaseWidget"/> class. </summary>
    public BaseWidget()
    {
        Painter = new GUIPainter(this);
    }

    /// <summary> Paints the widget. </summary>
    public virtual void Paint()
    {
        Painter.DrawRect(Rect);
    }
}
