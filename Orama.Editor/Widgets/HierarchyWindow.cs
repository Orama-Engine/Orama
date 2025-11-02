using Orama.Core.Common.Entities;
using Orama.Core.Modules;
using Orama.Core.Modules.GUI;
using Orama.Core.Modules.GUI.Layouts;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Scenes;
using Orama.Math;

namespace Orama.Editor.Widgets;

public class HierarchyWindow : EditorWindow
{
    /// <inheritdoc/>
    public override string Title => "Hierarchy";

    /// <summary> The currently selected entity. </summary>
    public Entity? SelectedEntity { get; set; }

    /// <summary> Occurs when a new entity is selected. </summary>
    public event Action? EntitySelected;

    public HierarchyWindow() : base()
    {
        foreach (var entity in ModuleManager.GetModule<SceneModule>()?.CurrentScene.Entities ?? Enumerable.Empty<Entity>())
        {
            Button button = new();
            button.Text = entity.Name;
            button.Clicked += () =>
            {
                SelectedEntity = entity;
                EntitySelected?.Invoke();
            };

            AddChild(button);
        }
    }
}
