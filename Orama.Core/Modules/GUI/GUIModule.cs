using Orama.Core.Common;
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

        Widget background = new();
        background.Rect = new Rect(290, 190, 120, 130);
        background.Style.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.25f);

        Button downButton = new();
        downButton.Text = "MOVE DOWN";
        downButton.Rect = new Rect(10, 70, 100, 50);
        downButton.Clicked += () =>
        {
            ModuleManager.GetModule<SceneModule>()?.CurrentScene.Entities.First().Transform.Position -= new Vector3(0, 1, 0);
        };


        Button upButton = new();
        upButton.Text = "MOVE UP";
        upButton.Rect = new Rect(10, 10, 100, 50);
        upButton.Clicked += () =>
        {
            ModuleManager.GetModule<SceneModule>()?.CurrentScene.Entities.First().Transform.Position += new Vector3(0, 1, 0);
        };

        background.AddChild(downButton);
        background.AddChild(upButton);

        Widgets.Add(background);
    }

    public override void Dispose()
    {
        Application.OnRender -= Render;
        ModuleManager.GetModule<InputModule>()?.MouseClicked -= CursorClick;
        ModuleManager.GetModule<InputModule>()?.MouseMoved -= UpdateCursorPosition;

        Widgets.Clear();
    }

    public void Render()
    {
        foreach (var widget in Widgets.Concat(Widgets.SelectMany(w => w.Children)))
            widget.Draw();
    }

    /// <summary> Register a GUI click. </summary>
    public void CursorClick(MouseButton button, Vector2 position)
    {
        if (button != MouseButton.Left)
            return;

        foreach (var widget in Widgets.Concat(Widgets.SelectMany(w => w.Children)))
        {
            if (widget.WorldRect.Contains(position))
                widget.OnClick();
        }
    }

    /// <summary> Register a GUI release. </summary>
    public void CursorRelease (MouseButton button, Vector2 position)
    {
        if (button != MouseButton.Left)
            return;

        foreach (var widget in Widgets.Concat(Widgets.SelectMany(w => w.Children)))
        {
            if (widget.WorldRect.Contains(position))
                widget.OnRelease();
        }
    }

    /// <summary> Sets the cursor position for GUI logic. </summary>
    public void UpdateCursorPosition(Vector2 position)
    {
        foreach (var widget in Widgets.Concat(Widgets.SelectMany(w => w.Children)))
        {
            bool contains = widget.WorldRect.Contains(position);
            if (contains && !widget.IsHovered)
                widget.OnPointerEnter();

            if (!contains && widget.IsHovered)
                widget.OnPointerExit();
        }
    }
}
