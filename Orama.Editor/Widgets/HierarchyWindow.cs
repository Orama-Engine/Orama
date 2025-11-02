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

    public HierarchyWindow() : base()
    {
        Rect = new Rect(200, 200, 300, 200);
        StyleNormal.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.25f);
        StyleNormal.Padding = 8;
        Layout = new VBoxLayout();
        Layout.Spacing = 4;

        foreach (var entity in ModuleManager.GetModule<SceneModule>()?.CurrentScene.Entities ?? Enumerable.Empty<Entity>())
        {
            Button button = new();
            button.Text = entity.Name;
            button.Clicked += () =>
            {
                entity.Transform.Position = new Vector3(0, 0, 0);
            };

            AddChild(button);
        }
    }
}
