using Orama.Math;
using Orama.Rendering;
using Orama.Resources.ResourceLibrary;
using Orama.Utils;

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
			Graphics.OnLoad();
		};

		Application.Update += () =>
		{
			SceneManager.Update();
			InputManager.Update(100.0f);
		};

		Application.Render += () =>
		{
			Graphics.OnRender(Matrix4x4.Identity, Matrix4x4.Identity);
		};

        Orama.Application.Run("Orama", 1000, 600, new DefaultResourceLibrary());
    }
}