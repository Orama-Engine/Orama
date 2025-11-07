using Orama.Core.Modules;
using Orama.Core.Modules.GUI;
using Orama.Core.Modules.GUI.Layouts;
using Orama.Core.Modules.GUI.Resources;
using Orama.Core.Modules.GUI.Styling;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Input;
using Orama.Math;

namespace Orama.Editor.Widgets;

/// <summary>
/// A Dockable Editor Window.
/// </summary>
public class EditorWindow : Widget
{
    /// <summary> The title of the window. </summary>
    public virtual string Title { get; set; } = "Editor Window";

    /// <summary> The rect of the window title bar. </summary>
    public Rect TitleRect => new Rect(Rect.X, Rect.Y, Rect.Width, Font.Default.MeasureText(Title).Y);

    private bool dragging;
    private float dragOffsetX;
    private float dragOffsetY;

    public EditorWindow()
    {
        StyleNormal.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.25f);
        StyleNormal.Padding = 8;
        Layout = new VBoxLayout();
        Layout.Spacing = 4;

        Label title = new Label(Title);
        AddChild(title);
    }

    /// <inheritdoc/>
    public override void Draw(Style style)
    {
        base.Draw(style);

        // Draw another rect behind the title
        Rect titleRect = new Rect(Rect.X, Rect.Y, Rect.Width, Font.Default.MeasureText(Title).Y + style.Padding);
        PaintEngine.DrawRect(ref titleRect, style.BackgroundColor);
    }

    /// <inheritdoc/>
    public override void OnClick()
    {
        base.OnClick();

        // Drag Window
        var input = ModuleManager.GetModule<InputModule>();
        if (input == null)
            return;

        if (!TitleRect.Contains(input.MousePosition))
            return;

        dragging = true;

        dragOffsetX = input.MousePosition.X - Rect.X;
        dragOffsetY = input.MousePosition.Y - Rect.Y;
    }

    /// <inheritdoc/>
    public override void OnRelease()
    {
        base.OnRelease();

        dragging = false;
    }

    /// <inheritdoc/>
    public override void OnPointerMove()
    {
        base.OnPointerMove();

        if (!dragging)
            return;

        var input = ModuleManager.GetModule<InputModule>();
        if (input == null)
            return;

        Rect = new Rect(
            input.MousePosition.X - dragOffsetX,
            input.MousePosition.Y - dragOffsetY,
            Rect.Width,
            Rect.Height
        );
    }
}
