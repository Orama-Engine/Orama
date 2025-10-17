

using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI.Styling;

/// <summary>
/// The default Orama GUI Theme.
/// </summary>
public class DefaultTheme : Theme
{
    /// <inheritdoc/>
    public override Dictionary<Type, Style> Styles { get; set; } = new()
    {
        { typeof(Widget), new Style() { BackgroundColor = new(1.0f, 1.0f, 1.0f, 1.0f) } },
    };
}
