
using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Module responsible for GUI.
/// </summary>
public class GUIModule : BaseModule
{
    public PaintEngine PaintEngine { get; set; } = new();

    public Widget RootWidget { get; set; } = new Widget();

    /// <inheritdoc/>
    public override void Update()
    {
        base.Update();

        RootWidget.Update();
        RootWidget.Draw(PaintEngine);
    }
}
