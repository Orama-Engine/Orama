using Orama.Common;
using Orama.GUI.Widgets;

namespace Orama.GUI;

/// <summary>
/// Module responsible for GUI.
/// </summary>
public class GUIModule : BaseModule
{
    public PaintEngine PaintEngine { get; set; } = new();

    public Widget RootWidget { get; set; } = new Widget();

    /// <inheritdoc/>
    public override void Initialize()
    {
        Application.OnUpdate += Update;
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        base.Dispose();

        Application.OnUpdate -= Update;
    }

    public void Update()
    {
        RootWidget.Update();
        RootWidget.Draw(PaintEngine);
    }
}
