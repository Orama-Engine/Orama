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
			Graphics.Initialize();
		};

		Application.Update += () =>
		{
			SceneManager.Update();
		};

		Application.Render += () =>
		{
			Graphics.EndFrame();
		};

        Orama.Application.Run("Orama", 1000, 600, new DefaultResourceLibrary());
    }
}