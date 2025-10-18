using Orama.Core.Common;
using Orama.Core.Common.Utility;
using Orama.Core.Modules;
using Orama.Core.Modules.Assemblies;
using Orama.Core.Modules.GUI;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Physics;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Scenes;

namespace Orama.Desktop;

internal class Program
{
    static void Main(string[] args)
    {
        // REGISTER MODULES
        ModuleManager.RegisterModule<AssemblyModule>();
        ModuleManager.RegisterModule<SceneModule>();
        ModuleManager.RegisterModule<RenderingModule>();
        ModuleManager.RegisterModule<GUIModule>();
        ModuleManager.RegisterModule<InputModule>();
        ModuleManager.RegisterModule<PhysicsModule>();

        Application.OnStart += () =>
        {
            EngineOutput.Log("Hello World!");
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
