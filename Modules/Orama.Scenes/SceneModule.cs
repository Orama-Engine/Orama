using Orama.Common;
using Orama.Scenes.Entities;
using Orama.Scenes.Resources;

namespace Orama.Scenes;

/// <summary>
/// Module responsible for managing scenes.
/// </summary>
public class SceneModule : BaseModule
{
    /// <summary> The currently loaded scene. </summary>
    public Scene CurrentScene { get; set; } = null!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        Application.OnUpdate += Update;

        EntityRegistry.RegisterFactories();

        CurrentScene = new Scene();
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        base.Dispose();

        Application.OnUpdate -= Update;
    }

    public void Update()
    {
        CurrentScene.UpdateAll();
    }
}
