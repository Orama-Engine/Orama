using Orama.Core.Common;
using Orama.Core.Common.Entities;
using Orama.Core.Common.Utility;
using Orama.Core.Modules;
using Orama.Core.Modules.Assemblies;
using Orama.Core.Modules.Audio;
using Orama.Core.Modules.Audio.Resources;
using Orama.Core.Modules.GUI;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Physics;
using Orama.Core.Modules.Physics.Components;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Rendering.Components;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Core.Modules.Scenes;
using Orama.Editor.Widgets;
using Orama.Math;

namespace Orama.Editor;

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
        ModuleManager.RegisterModule<EditorModule>();
        ModuleManager.RegisterModule<AudioModule>();

        Application.OnStart += () =>
        {
            EngineOutput.Log("Hello World!");

            Shader shader = Application.ResourceProvider.GetResource<Shader>("Assets/UnlitGeneric.shader") ?? throw new Exception("Failed to load UnlitGeneric shader!");

            FlyController flyController = new();
            flyController.Name = "Camera";
            flyController.Transform.Position = new Vector3(0, 2, 15);
            flyController.Start();

            Entity floor = new();
            MeshRenderer floorMR = new();
            floor.AddComponent(floorMR);
            floorMR.Mesh = Application.ResourceProvider.GetResource<Mesh>("Assets/PrimitiveCube.fbx");
            floorMR.Mesh?.Material = new(shader);
            floorMR.Mesh?.Material.SetParameter("Color", Color.White);
            floor.Name = "Floor";
            floor.Transform.Scale = new Vector3(10, 1, 10);
            floor.Transform.Position = new Vector3(0, 0, 0);
            var floorRb = new RigidBody();
            floorRb.IsStatic = true;
            var floorCollider = new BoxCollider(floor.Transform.Scale.X, floor.Transform.Scale.Y, floor.Transform.Scale.Z);
            floor.AddComponent(floorRb);
            floor.AddComponent(floorCollider);
            floor.Start();

            Entity cube = new();
            MeshRenderer cubeMR = new();
            cube.AddComponent(cubeMR);
            cubeMR.Mesh = Application.ResourceProvider.GetResource<Mesh>("Assets/PrimitiveCube.fbx");
            cubeMR.Mesh?.Material = new(shader);
            cubeMR.Mesh?.Material.SetParameter("Color", Color.White);
            cube.Name = "Cube";
            cube.Transform.Position = new Vector3(0, 100, 0);
            var cubeRb = new RigidBody();
            var cubeCollider = new BoxCollider(cube.Transform.Scale.X, cube.Transform.Scale.Y, cube.Transform.Scale.Z);
            cube.AddComponent(cubeRb);
            cube.AddComponent(cubeCollider);
            cube.Start();

            AudioClip test = Application.ResourceProvider.GetResource<AudioClip>("Assets/test.wav");
            IAudioSource source = ModuleManager.GetModule<AudioModule>().CreateSource();
            source.SetClip(test);
            source.Volume = 0.5f;
            source.Play();

            Label FPS = new("FPS: N/A");
            FPS.Rect = new Rect(5, 25, 0, 0);
            Application.OnRender += () => FPS.Text = $"FPS: {Application.Window.FramesPerSecond}";

            HierarchyWindow hierarchy = new HierarchyWindow();
            hierarchy.Rect = new Rect(200, 200, 300, 200);

            InspectorWindow inspector = new InspectorWindow();
            inspector.Rect = new Rect(500, 200, 300, 200);
            hierarchy.EntitySelected += () => inspector.Target = hierarchy.SelectedEntity;

            MenuBar menuBar = new MenuBar();
            MenuItem fileMenu = new MenuItem("File");
            fileMenu.OnClick += () => { EngineOutput.Log("File Menu Clicked"); };
            menuBar.AddMenuItem(fileMenu);

            MenuItem editMenu = new MenuItem("Edit");
            editMenu.OnClick += () => { EngineOutput.Log("Edit Menu Clicked"); };
            menuBar.AddMenuItem(editMenu);

            MenuItem viewMenu = new MenuItem("View");
            viewMenu.OnClick += () => { EngineOutput.Log("View Menu Clicked"); };
            menuBar.AddMenuItem(viewMenu);

            MenuItem settingsMenu = new MenuItem("Settings");
            settingsMenu.OnClick += () => { EngineOutput.Log("Settings Menu Clicked"); };
            menuBar.AddMenuItem(settingsMenu);

            MenuItem helpMenu = new MenuItem("Help");
            helpMenu.OnClick += () => { EngineOutput.Log("Help Menu Clicked"); };
            menuBar.AddMenuItem(helpMenu);

            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(FPS);
            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(hierarchy);
            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(inspector);
            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(menuBar);
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

