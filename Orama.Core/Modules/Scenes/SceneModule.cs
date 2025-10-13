using Orama.Core.Common.Components;
using Orama.Core.Common.Entities;
using Orama.Core.Modules.Scenes.Resources;
using Orama.Math;

namespace Orama.Core.Modules.Scenes;

/// <summary>
/// Module responsible for managing scenes.
/// </summary>
public class SceneModule : BaseModule
{
    /// <summary> The currently loaded scene. </summary>
    public Scene CurrentScene { get; set; } = null!;

    public override void Initialize()
    {
        CurrentScene = new Scene();

#if DEBUG
        BaseEntity camera = new BaseEntity();
        camera.AddComponent<Camera>();
        CurrentScene.Entities.Add(new DebugEntity());
        CurrentScene.Entities.Add(camera);
        camera.Transform.Position = new Vector3(0, 0, -3);
#endif

        CurrentScene.StartAll();
    }

    public override void Update()
    {
        CurrentScene.UpdateAll();
    }
}
