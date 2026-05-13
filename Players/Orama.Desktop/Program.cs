using Orama.Core.Common;
using Orama.Core.Common.Entities;
using Orama.Core.Modules;
using Orama.Core.Modules.Assemblies;
using Orama.Core.Modules.Audio;
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
        ModuleManager.RegisterModule<InputModule>();
        ModuleManager.RegisterModule<AudioModule>();

        ModuleManager.RegisterModule<RenderingModule>(); // Rendering should always be last

        var debugScene = new Scene();
        ModuleManager.GetModule<SceneModule>()?.CurrentScene = debugScene;


        Application.OnStart += () =>
        {
            FlyController flyController = new();
            flyController.Name = "Camera";
            flyController.Transform.Position = new Vector3(0, 10, 0);

            var floor = new DebugEntity();
            floor.Name = "Floor";
            floor.Transform.Scale = new Vector3(10, 1, 10);
            floor.Transform.Position = new Vector3(0, 0, 0);

            floor.Renderer.Material.SetParameter<Color>("u_Color", Color.White);


            ModuleManager.GetModule<SceneModule>()?.CurrentScene.StartAll();
        };

        Application.OnExit += () =>
        {

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
