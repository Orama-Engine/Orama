using Jitter2.Collision.Shapes;
using Jitter2.LinearMath;
using Orama.Core.Common.Entities;
using Orama.Core.Common.Utility;
using Orama.Core.Modules.Physics.Components;
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
        flyController.Name = "Camera";
        flyController.Transform.Position = new Vector3(0, 0, 0);
        CurrentScene.Entities.Add(flyController);

        var floor = new DebugEntity();
        floor.Name = "Floor";
        floor.Transform.Scale = new Vector3(10, 1, 10);
        floor.Transform.Position = new Vector3(0, 0, 0);
        JVector entSize = new JVector(floor.Transform.Scale.X, floor.Transform.Scale.Y, floor.Transform.Scale.Z);
        BoxShape entShape = new BoxShape(entSize);
        floor.AddComponent(new RigidBody(entShape, true));
        CurrentScene.Entities.Add(floor);

        var cube = new DebugEntity();
        cube.Name = "Cube";
        cube.Transform.Position = new Vector3(0, 100, 0);
        JVector cubeSize = new JVector(cube.Transform.Scale.X, cube.Transform.Scale.Y, cube.Transform.Scale.Z);
        BoxShape cubeShape = new BoxShape(cubeSize);
        cube.AddComponent(new RigidBody(cubeShape, false));
        cube.GetComponent<RigidBody>()?.AffectedByGravity = true;
        CurrentScene.Entities.Add(cube);
#endif

        CurrentScene.StartAll();
    }

    public override void Update()
    {
        CurrentScene.UpdateAll();
    }
}
