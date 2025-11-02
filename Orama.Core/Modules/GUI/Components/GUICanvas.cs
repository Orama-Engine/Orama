using Orama.Core.Common.Components;
using Orama.Core.Modules.GUI.Widgets;

namespace Orama.Core.Modules.GUI.Components;

/// <summary>
/// A GUI Rendered to screenspace.
/// </summary>
public class GUICanvas : Component
{
    /// <summary> The root widget to render. </summary>
    public virtual Widget? Root { get; set; }

    /// <inheritdoc/>
    public override void Start()
    {
        if (Root == null)
            return;

        ModuleManager.GetModule<GUIModule>()?.Widgets.Add(Root);
    }
}
