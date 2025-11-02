using Orama.Core.Modules.GUI.Layouts;
using Orama.Core.Modules.GUI.Styling;
using Orama.Core.Modules.Input;
using Orama.Math;


namespace Orama.Core.Modules.GUI.Widgets;

/// <summary>
/// Base class for all GUI widgets.
/// </summary>
public class Widget
{
    /// <summary> The position and size of the widget. </summary>
    public Rect Rect { get; set; }

    /// <summary> The state of the widget. </summary>
    public WidgetState State { get; set; }

    /// <summary> Whether the widget is currently focused. </summary>
    public bool IsFocused { get; set; }

    /// <summary> Determines how the widget sizes vertically. </summary>
    public SizePolicy VerticalSizePolicy { get; set; } = SizePolicy.Fixed;

    /// <summary> Determines how the widget sizes horizontally. </summary>
    public SizePolicy HorizontalSizePolicy { get; set; } = SizePolicy.Fixed;

    /// <summary> The layout of the widget. </summary>
    public Layout? Layout
    {
        get;
        set
        {
            field = value;
            value?.Parent = this;
        }
    }

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

    /// <summary> The styling of the widget in it's default state. </summary>
    public Style StyleNormal { get; set; }

    /// <summary> The styling of the widget in it's hovered state. </summary>
    public Style StyleHover { get; set; }

    /// <summary> The parent widget (if any). </summary>
    public Widget? Parent { get; set; }

    /// <summary> The child widgets. </summary>
    public IReadOnlyList<Widget> Children => children;

    /// <summary> Invoked when the <see cref="Rect"/> is clicked. </summary>
    public event Action? Clicked;

    /// <summary> Invoked when the <see cref="Rect"/> is released. </summary>
    public event Action? Released;

    /// <summary> Invoked when the <see cref="Rect"/> is first hovered. </summary>
    public event Action? PointerEntered;

    /// <summary> Invoked when the <see cref="Rect"/> is no longer hovered. </summary>
    public event Action? PointerExited;

    /// <summary> Invoked when the cursor is moved in the <see cref="Rect"/>. </summary>
    public event Action? PointerMoved;

    private List<Widget> children = new();

    /// <summary> Initializes a new instance of the <see cref="Widget"/> class. </summary>
    public Widget()
    {
        StyleNormal = ModuleManager.GetModule<GUIModule>()?.Theme.NormalStyles.GetValueOrDefault(GetType()) ?? new Style();
        StyleHover = ModuleManager.GetModule<GUIModule>()?.Theme.HoverStyles.GetValueOrDefault(GetType()) ?? StyleNormal;
    }

    /// <summary> Initializes a new instance of the <see cref="Widget"/> class. </summary>
    /// <param name="rect"> The position and size of the widget. </param>
    public Widget(Rect rect) : this()
    {
        Rect = rect;

        StyleNormal = ModuleManager.GetModule<GUIModule>()?.Theme.NormalStyles.GetValueOrDefault(GetType()) ?? new Style();
        StyleHover = ModuleManager.GetModule<GUIModule>()?.Theme.HoverStyles.GetValueOrDefault(GetType()) ?? StyleNormal;
    }

    /// <summary> Returns all child widgets and the widget itself. </summary>
    public IEnumerable<Widget> DescendantsAndSelf()
    {
        yield return this;

        foreach (Widget child in children)
        {
            foreach (var descendant in child.DescendantsAndSelf())
                yield return descendant;
        }
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
    /// <param name="style"> The style to use. </param>
    public virtual void Draw(Style style)
    {
        Rect refRect = WorldRect;
        PaintEngine.DrawRect(ref refRect, style.BackgroundColor);
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
        State = WidgetState.Hovered;
    }

    /// <summary> Runs when the <see cref="Rect"/> is no longer hovered. </summary>
    /// <remarks> The default implementation invokes the <see cref="PointerExited"/> event. </remarks>
    public virtual void OnPointerExit()
    {
        PointerExited?.Invoke();
        State = WidgetState.Normal;
    }

    /// <summary> Runs when the cursor is moved in the <see cref="Rect"/>. </summary>
    /// <remarks> The default implementation invokes the <see cref="PointerMoved"/> event. </remarks>
    public virtual void OnPointerMove() => PointerMoved?.Invoke();

    /// <summary> Runs when a key is pressed whilst the widget is focused. </summary>
    public virtual void OnKeyPress(Key key) { }
}

public enum WidgetState
{
    Normal,
    Hovered
}

/// <summary> Determines the size policy of a widget. </summary>
public enum SizePolicy
{
    /// <summary> The size of the widget does not change. </summary>
    Fixed,

    /// <summary> The size of the widget is expanded to fill available space. </summary>
    Expand
}