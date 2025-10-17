using Orama.Core.Common;
using Orama.Core.Common.Utility;
using Orama.Core.Modules.GUI.Styling;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Rendering;
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
        ModuleManager.GetModule<InputModule>()?.MouseMoved += UpdateCursorPosition;

        Widget myWidget = new();
        myWidget.Rect = new(0, 0, 100, 100);
        myWidget.Clicked += () => EngineOutput.Log("Clicked!");
        myWidget.PointerEntered += () => myWidget.Style = new Style() { BackgroundColor = new Color(1.0f, 0.0f, 0.0f, 1.0f) };
        myWidget.PointerExited += () => myWidget.Style = new Style() { BackgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };

        Widgets.Add(myWidget);
    }

    public void Render()
    {
        foreach (var widget in Widgets)
        {
            widget.Draw();

            foreach (var child in widget.Children)
                child.Draw();
        }
    }

    /// <summary> Register a GUI click. </summary>
    public void CursorClick(MouseButton button, Vector2 position)
    {
        if (button != MouseButton.Left)
            return;

        foreach (var widget in Widgets)
        {
            if (widget.Rect.Contains(position))
                widget.OnClick();
        }
    }

    /// <summary> Sets the cursor position for GUI logic. </summary>
    public void UpdateCursorPosition(Vector2 position)
    {
        foreach (var widget in Widgets)
        {
            bool contains = widget.Rect.Contains(position);
            if (contains && !widget.IsHovered)
                widget.OnPointerEnter();

            if (!contains && widget.IsHovered)
                widget.OnPointerExit();
        }
    }
}
