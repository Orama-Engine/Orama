using Orama.Core.Common;
using Orama.Core.Modules;
using Orama.Core.Modules.GUI;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Scenes;

namespace Orama.Desktop;

internal class Program
{
    static void Main(string[] args)
    {
        // REGISTER MODULES
        ModuleManager.RegisterModule<SceneModule>();
        ModuleManager.RegisterModule<RenderingModule>();
        ModuleManager.RegisterModule<InputModule>();
        ModuleManager.RegisterModule<GUIModule>();

        Application.OnStart += () =>
        {
            Console.WriteLine("Hello World!");
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
