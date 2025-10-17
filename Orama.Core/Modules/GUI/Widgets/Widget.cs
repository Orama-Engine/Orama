
namespace Orama.Core.Modules.GUI.Widgets;

/// <summary>
/// Base class for all GUI widgets.
/// </summary>
public class Widget
{
    /// <summary> The position and size of the widget. </summary>
    public Rect Rect { get; set; }

    /// <summary> The parent widget (if any). </summary>
    public Widget? Parent { get; set; }

    /// <summary> The child widgets (if any). </summary>
    public IReadOnlyList<Widget> Children => children;
    private List<Widget> children = new();

    /// <summary> Adds a child widget of type <typeparamref name="T"/>. </summary>
    public void AddChild<T>(T child) where T : Widget => AddChild(child);

    /// <summary> Adds a child widget. </summary>
    public void AddChild(Widget child)
    {
        children.Add(child);
        child.Parent = this;
    }

    /// <summary> Draws the widget using <see cref="PaintEngine"/>. </summary>
    public virtual void Draw()
    {
        Rect refRect = Rect;
        PaintEngine.DrawRect(ref refRect, new(1.0f, 1.0f, 1.0f, 1.0f));
    }
}
