using System.Numerics;
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
			Renderer.OnLoad();

			// Load a test scene
			Scene test = new();
			Entity ent = new();
			ent.AddComponent(new MeshRenderer());
			ent.AddComponent(new CameraController());
			test.Add(ent);

			SceneManager.LoadScene(test);
		};

		Application.Update += () =>
		{
			SceneManager.Update();
			Input.Update();
		};

		Application.Render += () =>
		{
			Renderer.OnRender(Camera.Main?.ViewMatrix ?? Matrix4x4.Identity, Camera.Main?.ProjectionMatrix ?? Matrix4x4.Identity);
		};

        Orama.Application.Run("Orama", 1000, 600, new DefaultResourceLibrary());
    }
}