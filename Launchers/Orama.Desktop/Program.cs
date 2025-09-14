using System.Numerics;
using Orama.Audio;
using Orama.Components;
using Orama.Entities;
using Orama.Physics;
using Orama.Rendering;
using Orama.Resources;
using Orama.Resources.ResourceLibrary;
using Orama.UserInput;
using Orama.Utility;

namespace Orama.Desktop;

/// <summary>
/// Default Launcher for Windows, Linux, Mac.
/// </summary>
class OramaDesktop
{
    static void Main(string[] args)
    {
		Application.Initialize += () =>
		{
			SceneManager.Initialize();
			Renderer.Initialize();
			AudioBackend.Initialize();
			RenderPipelineManager.Current.Initialize();
			PhysicsSystem.Initialize();

			InvokableAttribute.InvokeAll<OnGameInitializeAttribute>();

			// Load a test scene
			Scene test = new();
			Entity ent = new();
			Entity floor = new();
			ent.AddComponent(new MeshRenderer());
			 floor.AddComponent(new MeshRenderer()); // Temporarily hidden due to rendering issues for physics testing.

			ent.Transform.Position = new Vector3(0, 5, -5);
			floor.Transform.Position = new Vector3(0, -15, -5);
			floor.Transform.Scale = new Vector3(20, 1, 20);
			
			Collider floorCollider = new();
			floorCollider.Type = ColliderType.Box;
			floor.AddComponent(floorCollider);
			
			RigidBody floorRb = new();
			floorRb.Mass = 0f;
			floor.AddComponent(floorRb);
			
			Collider entCollider = new();
			entCollider.Type = ColliderType.Box;
			ent.AddComponent(entCollider);
			RigidBody entRb = new();
			ent.AddComponent(entRb);
			
			Console.WriteLine(ent.Transform.Matrix);
			test.Add(ent);
			test.Add(floor);

			Entity cam = new();
			cam.AddComponent(new CameraController());
			test.Add(cam);

			Application.ResourceLibrary.SaveResource<Scene>("Assets/testscene.orama", test);

			SceneManager.LoadScene(Application.ResourceLibrary.GetResource<Scene>("Assets/testscene.orama"));
		};

		Application.Update += () =>
		{
			SceneManager.Update();
			Input.Update();
		};

		Application.Render += () =>
		{
			RenderContext context = new();
			context.RenderingCamera = Camera.Main ?? new();
			RenderPipelineManager.RenderFrame(context);
		};

		Application.Quitting += () =>
		{
			InvokableAttribute.InvokeAll<OnGameQuitAttribute>();

			RenderPipelineManager.Current.Dispose();
			AudioBackend.Shutdown();
		};

        Orama.Application.Run("Orama", 1000, 600, new DefaultResourceLibrary());
    }
}