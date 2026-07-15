// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Assemblies;
using Orama.Audio;
using Orama.Common;
using Orama.Common.Utility;
using Orama.GUI;
using Orama.Input;
using Orama.Math;
using Orama.Physics;
using Orama.Rendering;
using Orama.Rendering.Entities;
using Orama.Scenes;
using Orama.Scenes.Entities;
using Orama.Scenes.Resources;
using Orama.VirtualReality;

namespace Orama.Desktop;

internal class Program
{
	static void Main(string[] args)
	{
		// Register all Modules our game wants to use
		ModuleManager.RegisterModule<InputModule>();
		ModuleManager.RegisterModule<AssemblyModule>();
		ModuleManager.RegisterModule<PhysicsModule>();
		ModuleManager.RegisterModule<SceneModule>();
		ModuleManager.RegisterModule<AudioModule>();
		ModuleManager.RegisterModule<GUIModule>();
		ModuleManager.RegisterModule<VirtualRealityModule>();
		ModuleManager.RegisterModule<RenderingModule>();

		var debugScene = new Scene();
		ModuleManager.GetModule<SceneModule>()?.CurrentScene = debugScene;

		Application.OnStart += () =>
		{
			ModuleManager.InitializeAll();

			// Setup debug scene
			Entity flyController = EntityRegistry.CreateEntity("fly_controller");
			flyController.Name = "Camera";
			flyController.Transform.Position = new Vector3(0, 0, 0);

			DebugEntity floor = EntityRegistry.CreateEntity<DebugEntity>("debug_entity");
			floor.Name = "Floor";
			floor.Transform.Scale = new Vector3(10, 1, 10);
			floor.Transform.Position = new Vector3(0, 0, 0);

			DebugEntity cube = EntityRegistry.CreateEntity<DebugEntity>("debug_entity");
			cube.Transform.Position = new Vector3(0, 1, 0);

			for (int i = 0; i < 1000; i++)
				OramaConsole.Log($"Running loop {i} times.");

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
