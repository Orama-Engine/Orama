using Orama.Core.Common;
using Orama.Core.Modules;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Scenes;
using Orama.Rendering;
using Orama.Rendering.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace Orama.Desktop;

internal class Program
{
    static void Main(string[] args)
    {
        // REGISTER MODULES
        ModuleManager.RegisterModule<SceneModule>();
        ModuleManager.RegisterModule<RenderingModule>();

        Application.OnStart += () =>
        {
            Console.WriteLine("Hello World!");
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
