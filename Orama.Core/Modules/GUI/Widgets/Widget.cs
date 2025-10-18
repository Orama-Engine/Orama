
using Orama.Core.Modules.GUI.Styling;
using Orama.Math;
using System.Runtime.InteropServices.JavaScript;

namespace Orama.Core.Modules.GUI.Widgets;

/// <summary>
/// Base class for all GUI widgets.
/// </summary>
public class Widget
{
    /// <summary> The position and size of the widget. </summary>
    public Rect Rect { get; set; }

    /// <summary> The position and size of the widget in world space. </summary>
    public Rect WorldRect
    {
        get
        {
            Vector2 worldPos = new Vector2(Rect.X, Rect.Y);
            Widget? current = Parent;

            while (current != null)
            {
                worldPos += new Vector2(current.Rect.X, current.Rect.Y);
                current = current.Parent;
            }

            return new Rect(worldPos.X, worldPos.Y, Rect.Width, Rect.Height);
        }
    }

    /// <summary> The desired size of the widget. </summary>
    public virtual Vector2 SizeHint => new Vector2(Rect.Width, Rect.Height);

    /// <summary> The styling of the widget. </summary>
    public Style Style { get; set; }

    /// <summary> The parent widget (if any). </summary>
    public Widget? Parent { get; set; }

    /// <summary> The child widgets. </summary>
    public IReadOnlyList<Widget> Children => children;

    /// <summary> Whether the widget is currently hovered. </summary>
    public bool IsHovered { get; private set; } = false;

    /// <summary> Invoked when the <see cref="Rect"/> is clicked. </summary>
    public event Action? Clicked;

    /// <summary> Invoked when the <see cref="Rect"/> is released. </summary>
    public event Action? Released;

    /// <summary> Invoked when the <see cref="Rect"/> is first hovered. </summary>
    public event Action? PointerEntered;

    /// <summary> Invoked when the <see cref="Rect"/> is no longer hovered. </summary>
    public event Action? PointerExited;

    private List<Widget> children = new();

    /// <summary> Initializes a new instance of the <see cref="Widget"/> class. </summary>
    public Widget() => Style = ModuleManager.GetModule<GUIModule>()?.Theme.Styles[GetType()] ?? new Style();

    /// <summary> Initializes a new instance of the <see cref="Widget"/> class. </summary>
    /// <param name="rect"> The position and size of the widget. </param>
    public Widget(Rect rect) : this()
    {
        Rect = rect;
        Style = ModuleManager.GetModule<GUIModule>()?.Theme.Styles[GetType()] ?? new Style();
    }

    /// <summary> Adds a new child widget of type <typeparamref name="T"/>. </summary>
    /// <typeparam name="T"> The type of the child widget to add. </typeparam>
    public void AddChild<T>() where T : Widget, new() => AddChild(new T());

    /// <summary> Adds a child widget. </summary>
    public void AddChild(Widget child)
    {
        children.Add(child);
        child.Parent = this;
    }

    /// <summary> Draws the widget using the <see cref="PaintEngine"/>. </summary>
    public virtual void Draw()
    {
        Rect refRect = WorldRect;
        if (IsHovered)
        {
            PaintEngine.DrawRect(ref refRect, Style.HoverBackgroundColor ?? Style.BackgroundColor);
            return;
        }

        PaintEngine.DrawRect(ref refRect, Style.BackgroundColor);
    }

    /// <summary> Runs when the <see cref="Rect"/> is clicked. </summary>
    /// <remarks> The default implementation invokes the <see cref="Clicked"/> event. </remarks>
    public virtual void OnClick() => Clicked?.Invoke();

    /// <summary> Runs when the <see cref="Rect"/> is released. </summary>
    /// <remarks> The default implementation invokes the <see cref="Released"/> event. </remarks>
    public virtual void OnRelease() => Released?.Invoke();

    /// <summary> Runs when the <see cref="Rect"/> is first hovered. </summary>
    /// <remarks> The default implementation invokes the <see cref="PointerEntered"/> event. </remarks>
    public virtual void OnPointerEnter()
    {
        PointerEntered?.Invoke();
        IsHovered = true;
    }

    /// <summary> Runs when the <see cref="Rect"/> is no longer hovered. </summary>
    /// <remarks> The default implementation invokes the <see cref="PointerExited"/> event. </remarks>
    public virtual void OnPointerExit()
    {
        PointerExited?.Invoke();
        IsHovered = false;
    }
}
