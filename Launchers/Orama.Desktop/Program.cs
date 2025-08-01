using System.Numerics;
using Orama.Audio;
using Orama.Components;
using Orama.Engine;
using Orama.Entities;
using Orama.Rendering;
using Orama.Resources;
using Orama.Resources.ResourceLibrary;
using Orama.UserInput;

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

			// Load a test scene
			Scene test = new();
			Entity ent = new();
			ent.AddComponent(new MeshRenderer());
			test.Add(ent);

			Entity cam = new();
			cam.AddComponent(new CameraController());
			test.Add(cam);

			Application.ResourceLibrary.SaveResource<Scene>("Assets/testscene.orama", test);

			SceneManager.LoadScene(Application.ResourceLibrary.GetResource<Scene>("Assets/testscene.orama"));
			
			AudioManager.PlaySound("Assets/scream.wav");
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
			AudioBackend.Shutdown();
		};

        Orama.Application.Run("Orama", 1000, 600, new DefaultResourceLibrary());
    }
}