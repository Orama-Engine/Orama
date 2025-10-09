using Orama.Core;

namespace Orama.Desktop;

internal class Program
{
    static void Main(string[] args)
    {
        Application.OnStart += () =>
        {
            Console.WriteLine("Hello World!");
        };

        Application.OnUpdate += () =>
        {

        };

        Application.OnExit += () =>
        {

        };

        Application.Initialize();
    }
}
