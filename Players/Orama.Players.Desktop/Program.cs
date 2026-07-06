using Orama.Rendering;
using Orama.Math;
using Orama.Input;
using Orama.Scenes;
using Orama.Assemblies;
using Orama.Audio;
using Orama.GUI;
using Orama.Physics;
using Orama.Scenes.Entities;
using Orama.Scenes.Resources;
using Orama.Common;
using Orama.Common.Utility;
using Orama.VirtualReality;
using Orama.Rendering.Entities;

namespace Orama.Desktop;

internal class Program
{
    static void Main(string[] args)
    {
        // REGISTER MODULES
        ModuleManager.RegisterModule<AssemblyModule>();

        ModuleManager.RegisterModule<PhysicsModule>();
        ModuleManager.RegisterModule<SceneModule>();
        ModuleManager.RegisterModule<AudioModule>();
        ModuleManager.RegisterModule<GUIModule>();

        ModuleManager.RegisterModule<InputModule>();
        ModuleManager.RegisterModule<RenderingModule>();
        ModuleManager.RegisterModule<VirtualRealityModule>();

        var debugScene = new Scene();
        ModuleManager.GetModule<SceneModule>()?.CurrentScene = debugScene;

        Application.OnStart += () =>
        {
            ModuleManager.InitializeAll();

            Entity flyController = EntityRegistry.CreateEntity("fly_controller");
            flyController.Name = "Camera";
            flyController.Transform.Position = new Vector3(0, 0, 0);

            DebugEntity floor = EntityRegistry.CreateEntity<DebugEntity>("debug_entity");
            floor.Name = "Floor";
            floor.Transform.Scale = new Vector3(10, 1, 10);
            floor.Transform.Position = new Vector3(0, 0, 0);

            ModuleManager.GetModule<SceneModule>()?.CurrentScene.StartAll();
        };

        Application.OnExit += () =>
        {
            ModuleManager.DisposeAll();
        };

        Application.OnUpdate += () =>
        {

        };

        Application.OnRender += () =>
        {

        };

        Application.Initialize();
    }
}
