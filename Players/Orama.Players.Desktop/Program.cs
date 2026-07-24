// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Assemblies;
using Orama.Audio;
using Orama.Common;
using Orama.GUI;
using Orama.Input;
using Orama.Math;
using Orama.Physics;
using Orama.Rendering;
using Orama.Rendering.Components;
using Orama.Rendering.Entities;
using Orama.Rendering.Resources;
using Orama.Scenes;
using Orama.Scenes.Entities;
using Orama.VirtualReality;

namespace Orama.Desktop;

internal sealed class Program
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

		Application.OnStart += () =>
		{
			// Setup debug scene
			FlyController flyController = new();
			flyController.Name = "Camera";

			DebugEntity floor = new();
			floor.Name = "Floor";
			floor.Transform.Scale = new Vector3(10, 1, 10);

			Mesh? cubeMesh = Application.ResourceProvider.GetResource<Mesh>("Assets/PrimitiveCube.fbx");
			Shader? shader = Application.ResourceProvider.GetResource<Shader>("Assets/Orama/Unlit.slang");
			cubeMesh?.Material = new(shader ?? throw new Exception("Shader not found"));

			Entity cube = new();
			cube.Transform.Position = new Vector3(0, 2, 0);
			MeshRenderer renderer = cube.AddComponent<MeshRenderer>();
			renderer.Mesh = cubeMesh;

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
