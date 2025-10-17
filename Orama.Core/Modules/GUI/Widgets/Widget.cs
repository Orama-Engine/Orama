
using Orama.Core.Modules.GUI.Styling;

namespace Orama.Core.Modules.GUI.Widgets;

/// <summary>
/// Base class for all GUI widgets.
/// </summary>
public class Widget
{
    /// <summary> The position and size of the widget. </summary>
    public Rect Rect { get; set; }

    /// <summary> The styling of the widget. </summary>
    public Style Style { get; set; } = ModuleManager.GetModule<GUIModule>()?.Theme.Styles[typeof(Widget)] ?? new();

    /// <summary> The parent widget (if any). </summary>
    public Widget? Parent { get; set; }

    /// <summary> The child widgets. </summary>
    public IReadOnlyList<Widget> Children => children;

    /// <summary> Whether the widget is currently hovered. </summary>
    public bool IsHovered { get; private set; } = false;

    /// <summary> Invoked when the <see cref="Rect"/> is clicked. </summary>
    public event Action? Clicked;

    /// <summary> Invoked when the <see cref="Rect"/> is first hovered. </summary>
    public event Action? PointerEntered;

    /// <summary> Invoked when the <see cref="Rect"/> is no longer hovered. </summary>
    public event Action? PointerExited;

    private List<Widget> children = new();

    /// <summary> Adds a new child widget of type <typeparamref name="T"/>. </summary>
    /// <typeparam name="T"> The type of the child widget to add. </typeparam>
    public void AddChild<T>() where T : Widget, new() => AddChild(new T());

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
        PaintEngine.DrawRect(ref refRect, Style.BackgroundColor);
    }

    /// <summary> Invoked when the <see cref="Rect"/> is clicked. </summary>
    /// <remarks> The default implementation invokes the <see cref="Clicked"/> event. </remarks>
    public virtual void OnClick() => Clicked?.Invoke();

    /// <summary> Invoked when the <see cref="Rect"/> is first hovered. </summary>
    /// <remarks> The default implementation invokes the <see cref="PointerEntered"/> event. </remarks>
    public virtual void OnPointerEnter()
    {
        PointerEntered?.Invoke();
        IsHovered = true;
    }

    /// <summary> Invoked when the <see cref="Rect"/> is no longer hovered. </summary>
    /// <remarks> The default implementation invokes the <see cref="PointerExited"/> event. </remarks>
    public virtual void OnPointerExit()
    {
        PointerExited?.Invoke();
        IsHovered = false;
    }
}
