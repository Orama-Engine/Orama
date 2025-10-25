

using Orama.Math;

namespace Orama.Core.Modules.GUI.Styling;

/// <summary>
/// A Collection of style properties.
/// </summary>
public class Style
{
    /// <summary> The background color. </summary>
    public Color BackgroundColor { get; set; } = Color.Transparent;

    /// <summary> The color of text. </summary>
    public Color TextColor { get; set; } = Color.White;

    /// <summary> The padding. </summary>
    public float Padding { get; set; }
}
