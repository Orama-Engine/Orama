using Orama.Core.Common;
using Orama.Core.Common.Utility;
using Orama.Core.Modules.GUI.Layouts;
using Orama.Core.Modules.GUI.Styling;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Scenes;
using Orama.Math;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Module responsible for handling GUI.
/// </summary>
public class GUIModule : BaseModule
{
    /// <summary> List of all widgets currently active. </summary>
    public List<Widget> Widgets { get; } = new();

    /// <summary> The current GUI theme. </summary>
    public Theme Theme { get; set; } = new DefaultTheme();

    public override void Initialize()
    {
        Application.OnRender += Render;

        ModuleManager.GetModule<InputModule>()?.MouseClicked += CursorClick;
        ModuleManager.GetModule<InputModule>()?.MouseReleased += CursorRelease;
        ModuleManager.GetModule<InputModule>()?.MouseMoved += UpdateCursorPosition;
        ModuleManager.GetModule<InputModule>()?.KeyPressed += KeyPress;

        Label FPS = new("FPS: 0");
        Application.OnUpdate += () => FPS.Text = $"FPS: {Application.Window.FramesPerSecond}";

        Widget background = new();
        background.Rect = new Rect(200, 200, 300, 200);
        background.StyleNormal.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.25f);
        background.StyleNormal.Padding = 8;
        background.Layout = new VBoxLayout();
        background.Layout.Spacing = 4;

        Label label = new("Test Label");
        background.AddChild(label);

        Button button = new();
        button.Text = "Test Button";
        button.Clicked += () =>
        {
            EngineOutput.Log("Button clicked!");
        };

        background.AddChild(button);

        Widgets.Add(background);
        Widgets.Add(FPS);
    }

    public override void Dispose()
    {
        Application.OnRender -= Render;
        ModuleManager.GetModule<InputModule>()?.MouseClicked -= CursorClick;
        ModuleManager.GetModule<InputModule>()?.MouseMoved -= UpdateCursorPosition;

        Widgets.Clear();
    }

    public override void Update()
    {
        LayoutEngine.LayoutWidgets(Widgets);
    }

    public void Render()
    {
        foreach (var widget in Widgets.Concat(Widgets.SelectMany(w => w.DescendantsAndSelf())))
        {
            if (widget.State == WidgetState.Normal)
                widget.Draw(widget.StyleNormal);

            if (widget.State == WidgetState.Hovered)
                widget.Draw(widget.StyleHover);
        }
    }

    /// <summary> Register a GUI click. </summary>
    public void CursorClick(MouseButton button, Vector2 position)
    {
        if (button != MouseButton.Left)
            return;

        foreach (var widget in Widgets.Concat(Widgets.SelectMany(w => w.DescendantsAndSelf())))
        {
            bool contains = widget.WorldRect.Contains(position);
            widget.IsFocused = contains;
            if (contains)
                widget.OnClick();
        }
    }

    /// <summary> Register a GUI release. </summary>
    public void CursorRelease(MouseButton button, Vector2 position)
    {
        if (button != MouseButton.Left)
            return;

        foreach (var widget in Widgets.Concat(Widgets.SelectMany(w => w.DescendantsAndSelf())))
        {
            if (widget.WorldRect.Contains(position))
                widget.OnRelease();
        }
    }

    /// <summary> Sets the cursor position for GUI logic. </summary>
    public void UpdateCursorPosition(Vector2 position)
    {
        foreach (var widget in Widgets.Concat(Widgets.SelectMany(w => w.DescendantsAndSelf())))
        {
            bool contains = widget.WorldRect.Contains(position);
            if (contains && widget.State != WidgetState.Hovered)
                widget.OnPointerEnter();

            if (!contains && widget.State == WidgetState.Hovered)
                widget.OnPointerExit();
        }
    }

    /// <summary> Register a key press. </summary>
    public void KeyPress(Key key)
    {
        foreach (var widget in Widgets.Concat(Widgets.SelectMany(w => w.DescendantsAndSelf())))
        {
            if (widget.IsFocused)
                widget.OnKeyPress(key);
        }
    }
}
