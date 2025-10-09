using Orama.Core;
using Orama.Rendering;

namespace Orama.Desktop;

internal class Program
{
    static void Main(string[] args)
    {
        Application.OnStart += () =>
        {
            Console.WriteLine("Hello World!");
            Renderer.Initialize(Application.Window.InternalWindow, RendererBackend.OpenGL);
        };

        Application.OnExit += () =>
        {

        };

        Application.OnUpdate += () =>
        {

        };

        Application.OnRender += () =>
        {
            Renderer.Render();
        };

        Application.Initialize();
    }
}
