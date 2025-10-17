using Orama.Core.Common;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Rendering;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Module responsible for handling GUI.
/// </summary>
public class GUIModule : BaseModule
{
    public override HashSet<Type> Dependencies { get; } = new() { typeof(RenderingModule) };

    /// <summary> List of all widgets currently active. </summary>
    public List<Widget> Widgets { get; } = new();

    public override void Initialize()
    {
        Application.OnRender += Render;

        Widget myWidget = new();
        myWidget.Rect = new(0, 0, 100, 100);
        Widgets.Add(myWidget);
    }

    public void Render()
    {
        foreach (var widget in Widgets)
        {
            widget.Draw();

            foreach (var child in widget.Children)
                child.Draw();
        }
    }
}
