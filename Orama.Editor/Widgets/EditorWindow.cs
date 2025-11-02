using Orama.Core.Modules;
using Orama.Core.Modules.GUI;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Input;
using Orama.Math;

namespace Orama.Editor.Widgets;

/// <summary>
/// A Dockable Editor Window.
/// </summary>
public class EditorWindow : Widget
{
    private bool _dragging;
    private float _dragOffsetX;
    private float _dragOffsetY;

    /// <inheritdoc/>
    public override void OnClick()
    {
        base.OnClick();

        // Drag Window
        var input = ModuleManager.GetModule<InputModule>();
        if (input == null) return;

        _dragging = true;

        _dragOffsetX = input.MousePosition.X - Rect.X;
        _dragOffsetY = input.MousePosition.Y - Rect.Y;
    }

    /// <inheritdoc/>
    public override void OnRelease()
    {
        base.OnRelease();

        _dragging = false;
    }

    /// <inheritdoc/>
    public override void OnPointerMove()
    {
        base.OnPointerMove();

        if (!_dragging)
            return;

        var input = ModuleManager.GetModule<InputModule>();
        if (input == null) return;

        Rect = new Rect(
            input.MousePosition.X - _dragOffsetX,
            input.MousePosition.Y - _dragOffsetY,
            Rect.Width,
            Rect.Height
        );
    }
}
