using Orama.Core.Common.Utility;
using Orama.Core.Modules.GUI.Resources;
using Orama.Core.Modules.GUI.Styling;
using Orama.Core.Modules.Input;
using Orama.Math;

namespace Orama.Core.Modules.GUI.Widgets;

public class LineEdit : Widget
{
    /// <summary> The text inputted by the user. </summary>
    public string Text { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override Vector2 SizeHint => Font.Default.MeasureText(Text);

    public override void Draw(Style style)
    {
        base.Draw(style);

        PaintEngine.DrawText(Text, new Vector2(WorldRect.X, WorldRect.Y), style.TextColor, Font.Default);
    }

    public override void OnKeyPress(Key key)
    {
        if (key >= Key.A && key <= Key.Z)
            Text += key.ToString();

        if (key == Key.Backspace && Text.Length > 0)
            Text = Text.Substring(0, Text.Length - 1);
    }
}
