
using Orama.Rendering;

namespace Orama.Core;

/// <summary>
/// The top-level application.
/// </summary>
public static class Application
{
    /// <summary> Called when the application starts. </summary>
    public static Action? OnStart { get; set; }

    /// <summary> Called when the application exits. </summary>
    public static Action? OnExit { get; set; }

    /// <summary> Called when the application updates. </summary>
    public static Action? OnUpdate { get; set; }

    /// <summary> The main window. </summary>
    public static Window Window { get; private set; } = null!;

    public static void Initialize()
    {
        Window = new Window();

        Renderer.Initialize(Window.InternalWindow, RendererBackend.OpenGL);

        Window.InternalWindow.Load += () => OnStart?.Invoke();
        Window.InternalWindow.Closing += () => OnExit?.Invoke();
        Window.InternalWindow.Update += (double delta) => 
        {
            Time.Delta = (float)delta;
            Time.PreciseDelta = delta;

            OnUpdate?.Invoke();
        };

        Window.Run();
    }
}
