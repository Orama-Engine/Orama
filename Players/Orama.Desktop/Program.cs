using Orama.Core.Common;
using Orama.Core.Common.Entities;
using Orama.Core.Modules;
using Orama.Core.Modules.Assemblies;
using Orama.Core.Modules.GUI;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Physics;
using Orama.Core.Modules.Physics.Components;
using Orama.Core.Modules.Physics.Components.Colliders;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Scenes;
using Orama.Core.Modules.Scenes.Resources;
using Orama.Math;
using System.Text;

namespace Orama.Desktop;

internal class Program
{
    static void Main(string[] args)
    {
        // REGISTER MODULES
        ModuleManager.RegisterModule<AssemblyModule>();
        ModuleManager.RegisterModule<PhysicsModule>();
        ModuleManager.RegisterModule<SceneModule>();
        ModuleManager.RegisterModule<RenderingModule>();
        ModuleManager.RegisterModule<GUIModule>();
        ModuleManager.RegisterModule<InputModule>();

        var debugScene = new Scene();
        ModuleManager.GetModule<SceneModule>()?.CurrentScene = debugScene;

        var FPS = new Label("FPS: N/A");
        FPS.Rect = new Rect(5, 5, 0, 0);

        Application.OnStart += () =>
        {
            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(FPS);

            Entity test = new();
            test.Transform.Position = new Vector3(0, 5, 0);
            byte[] serialized = Serialization.Serialization.Serialize(test);
            Console.WriteLine(Encoding.UTF8.GetString(serialized));
            test.Destroy();

            Entity deserialized = Serialization.Serialization.Deserialize<Entity>(serialized);
            Console.WriteLine(deserialized.Name);
            Console.WriteLine(deserialized.Transform.Position);
 

            FlyController flyController = new();
            flyController.Name = "Camera";
            flyController.Transform.Position = new Vector3(0, 10, 0);

            var floor = new DebugEntity();
            floor.Name = "Floor";
            floor.Transform.Scale = new Vector3(10, 1, 10);
            floor.Transform.Position = new Vector3(0, 0, 0);


            var cube = new DebugEntity();
            cube.Name = "Cube";
            cube.Transform.Position = new Vector3(0, 100, 0);
            var cubeRb = new RigidBody();
            var cubeCollider = new BoxCollider(cube.Transform.Scale.X, cube.Transform.Scale.Y, cube.Transform.Scale.Z);
            cube.AddComponent(cubeRb);
            cube.AddComponent(cubeCollider);

            ModuleManager.GetModule<SceneModule>()?.CurrentScene.StartAll();
        };

        Application.OnExit += () =>
        {

        };

        Application.OnUpdate += () =>
        {
            FPS.Text = $"FPS: {Application.Window.FramesPerSecond}";
        };

        Application.OnRender += () =>
        {

        };

        Application.Initialize();
    }
}
