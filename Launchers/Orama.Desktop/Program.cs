using System.Numerics;
using Orama.Audio;
using Orama.Components;
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

			Application.ResourceLibrary.GetResource<Resources.Audio>("Assets/scream.wav").Play();
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
			RenderPipelineManager.Current.Dispose();
			AudioBackend.Shutdown();
		};

        Orama.Application.Run("Orama", 1000, 600, new DefaultResourceLibrary());
    }
}