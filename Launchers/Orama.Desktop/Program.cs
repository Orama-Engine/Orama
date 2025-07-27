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
		Application.Update += () =>
		{
			SceneManager.Update();
		};

        Orama.Application.Run("Orama", 1000, 600, new DefaultResourceLibrary());
    }
}