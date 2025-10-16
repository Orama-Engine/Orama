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
        FlyController flyController = new FlyController();
        flyController.Transform.Position = new Vector3(0, 0, 0);
        CurrentScene.Entities.Add(flyController);
        CurrentScene.Entities.Add(new DebugEntity());
#endif

        CurrentScene.StartAll();
    }

    public override void Update()
    {
        CurrentScene.UpdateAll();
    }
}
