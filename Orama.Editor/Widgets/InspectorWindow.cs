

using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Editor.Widgets;

public class InspectorWindow : EditorWindow
{
    /// <inheritdoc/>
    public override string Title => "Inspector";

    /// <summary> The target object to show the properties of. </summary>
    public object? Target 
    { 
        get; 
        set
        {
            field = value;
            targetType.Text = value?.GetType().Name ?? "null";
        }
    }

    private Label targetType;

    public InspectorWindow() : base()
    {
        targetType = new("Type");
        AddChild(targetType);
    }
}
