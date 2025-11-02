using Orama.Core.Common;
using Orama.Core.Common.Utility;
using Orama.Core.Modules;
using Orama.Core.Modules.Assemblies;
using Orama.Core.Modules.GUI;
using Orama.Core.Modules.GUI.Layouts;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Physics;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Scenes;
using Orama.Editor.Modules.Editor;
using Orama.Math;

namespace Orama.Editor;

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

        ModuleManager.RegisterModule<EditorModule>();

        Application.OnStart += () =>
        {
            EngineOutput.Log("Hello World!");

            Label FPS = new("FPS: N/A");
            FPS.Rect = new Rect(5, 5, 0, 0);
            Application.OnRender += () => FPS.Text = $"FPS: {Application.Window.FramesPerSecond}";

            Widget background = new();
            background.Rect = new Rect(200, 200, 300, 200);
            background.StyleNormal.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.25f);
            background.StyleNormal.Padding = 8;
            background.Layout = new VBoxLayout();
            background.Layout.Spacing = 4;

            Label label = new("Test Label");
            background.AddChild(label);

            Button button = new();
            button.Text = "Test Button";
            button.Clicked += () =>
            {
                EngineOutput.Log("Button clicked!");
            };

            background.AddChild(button);

            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(background);
            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(FPS);
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

