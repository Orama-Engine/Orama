using Orama.Core.Modules.GUI.Resources;
using Orama.Core.Modules.GUI.Styling;
using Orama.Math;

namespace Orama.Core.Modules.GUI.Widgets;

/// <summary>
/// Widget used to display text.
/// </summary>
public class Label : Widget
{
    /// <summary> The text of the label. </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary> Initializes a new instance of the <see cref="Label"/> class. </summary>
    public Label() { }

    /// <summary> Initializes a new instance of the <see cref="Label"/> class with the specified text. </summary>
    public Label(string text) => Text = text;

    /// <inheritdoc/>
    public override Vector2 SizeHint => Font.Default.MeasureText(Text);

    public override void Draw(Style style)
    {
        base.Draw(style);

        PaintEngine.DrawText(Text, new Vector2(WorldRect.X, WorldRect.Y), Color.White, Font.Default);
    }
}
