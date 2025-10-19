
using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI.Layouts;

/// <summary>
/// Base class for Layouts.
/// </summary>
public abstract class Layout
{
    /// <summary> The spacing between layouted widgets. </summary>
    public int Spacing { get; set; } = 5;

    /// <summary> The owning widget. </summary>
    public Widget? Parent { get; set; }

    /// <summary> Layouts widgets. </summary>
    public abstract void LayoutChildren();
}
