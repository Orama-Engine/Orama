using Orama.Resources;

namespace Orama.Desktop;

/// <summary>
/// Default Launcher for Windows, Linux, Mac.
/// </summary>
class OramaDesktop
{
    static void Main(string[] args)
    {
        Orama.Application.Run("Orama", 1000, 600, new DefaultResourceLibrary());
    }
}