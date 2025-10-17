using Orama.Core.Common;
using Orama.Core.Common.Utility;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Rendering;
using Orama.Math;

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

        ModuleManager.GetModule<InputModule>()?.MouseClicked += Click;

        Widget myWidget = new();
        myWidget.Rect = new(0, 0, 100, 100);
        myWidget.Clicked += () => EngineOutput.Log("Clicked!");
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

    /// <summary> Register a GUI click. </summary>
    public void Click(MouseButton button, Vector2 position)
    {
        if (button != MouseButton.Left)
            return;

        foreach (var widget in Widgets)
        {
            if (widget.Rect.Contains(position))
                widget.OnClick();
        }
    }
}
