using Orama.Core.Common.Resources;
using Orama.Core.Common.Resources.Default;
using Orama.Math;

namespace Orama.Core.Common;

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

    /// <summary> Called when the application renders. </summary>
    public static Action? OnRender { get; set; }

    /// <summary> Called when the application resizes. </summary>
    public static Action<Vector2>? OnResize { get; set; }

    /// <summary> The resource provider. </summary>
    public static IResourceProvider ResourceProvider { get; set; } = null!;

    /// <summary> The main window. </summary>
    public static Window Window { get; private set; } = null!;

    public static void Initialize(IResourceProvider? resourceProvider = null)
    {
        if (resourceProvider == null)
            resourceProvider = new DefaultResourceProvider();

        ResourceProvider = resourceProvider;

        Window = new Window();

        Window.InternalWindow.Load += () => OnStart?.Invoke();
        Window.InternalWindow.Closing += () => OnExit?.Invoke();
        Window.InternalWindow.Render += (delta) => OnRender?.Invoke();
        Window.InternalWindow.Resize += (size) => OnResize?.Invoke(new Vector2(size.X, size.Y));
        Window.InternalWindow.Update += (delta) => 
        {
            Time.Delta = (float)delta;
            Time.PreciseDelta = delta;

            OnUpdate?.Invoke();
        };

        Window.Run();
    }
}
