
using Orama.Core.Modules.GUI.Resources;
using Orama.Math;

namespace Orama.Core.Modules.GUI.Widgets;

/// <summary>
/// A button widget.
/// </summary>
public class Button : Widget
{
    /// <summary> The text displayed on the button. </summary>
    public string Text { get; set; } = "Button";

    /// <inheritdoc/>
    public override Vector2 SizeHint => Font.Default.MeasureText(Text);

    public override void Draw()
    {
        base.Draw();

        PaintEngine.DrawText(Text, new Vector2(WorldRect.X, WorldRect.Y), Color.White, Font.Default);
    }
}
