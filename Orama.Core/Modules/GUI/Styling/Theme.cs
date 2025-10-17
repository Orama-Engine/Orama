using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI.Styling;

/// <summary>
/// A Collection of style properties.
/// </summary>
public abstract class Theme
{
    /// <summary> A Collection of <see cref="Widget"/>s mapped to their default <see cref="Style"/>. </summary>
    public virtual Dictionary<Type, Style> Styles { get; set; } = new Dictionary<Type, Style>();
}
