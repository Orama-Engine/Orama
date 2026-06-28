using Orama.Math;

namespace Orama.GUI.Widgets;

/// <summary>
/// Base class for all GUI widgets.
/// </summary>
public class Widget
{
    public Rect Rect { get; set; }

    public virtual void Update()
    {

    }

    public virtual void Draw(PaintEngine pEngine)
    {
        pEngine.DrawRect(Rect, Color.White);
    }
}
