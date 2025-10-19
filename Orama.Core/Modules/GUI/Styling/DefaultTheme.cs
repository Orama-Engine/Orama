using Orama.Math;
using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI.Styling;

/// <summary>
/// The default Orama GUI Theme.
/// </summary>
public class DefaultTheme : Theme
{
    /// <inheritdoc/>
    public override Dictionary<Type, Style> NormalStyles { get; set; } = new()
    {
        { typeof(Widget), new Style() { BackgroundColor = new(1.0f, 1.0f, 1.0f, 1.0f) } },
        { typeof(Button), new Style() { BackgroundColor = new(0.15f, 0.29f, 0.44f, 0.5f) } },
        { typeof(Label), new Style() {} }
    };

    public override Dictionary<Type, Style> HoverStyles { get; set; } = new()
    {
        { typeof(Button), new Style() { BackgroundColor = new(0.26f, 0.59f, 0.98f, 0.80f) } },
        { typeof(Label), new Style() {} }
    };
}
