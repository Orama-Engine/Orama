using Orama.Core.Common.Entities;
using Orama.Core.Modules;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Scenes;
using Orama.Editor.Entities;


namespace Orama.Editor.Widgets;

public class HierarchyWindow : EditorWindow
{
    /// <inheritdoc/>
    public override string Title => "Hierarchy";

    /// <summary> The currently selected entity. </summary>
    public Entity? SelectedEntity { get; set; }

    /// <summary> Occurs when a new entity is selected. </summary>
    public event Action? EntitySelected;

    private Gizmo3D gizmo;

    public HierarchyWindow() : base()
    {
        gizmo = new Gizmo3D();
        gizmo.Name = "Gizmo3D";
        gizmo.Type = GizmoType.Translate;
        gizmo.Start();

        foreach (var entity in ModuleManager.GetModule<SceneModule>()?.CurrentScene.Entities ?? Enumerable.Empty<Entity>())
        {
            Button button = new();
            button.Text = entity.Name;
            button.Clicked += () =>
            {
                SelectedEntity = entity;
                EntitySelected?.Invoke();

                gizmo.Transform.Position = entity.Transform.Position;
            };

            AddChild(button);
        }
    }
}
