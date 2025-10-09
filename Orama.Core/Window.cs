using Silk.NET.Windowing;

namespace Orama.Core;

/// <summary>
/// A Platform-agnostic Window.
/// </summary>
public class Window
{
    /// <summary> The internal Silk.NET window. </summary>
    public IWindow InternalWindow { get; }

    /// <summary> Initializes a new instance of the <see cref="Window"/> class. </summary>
    public Window()
    {
        WindowOptions options = WindowOptions.Default;
        InternalWindow = Silk.NET.Windowing.Window.Create(options);
    }

    /// <summary> Starts the window loop. </summary>
    public void Run() => InternalWindow.Run();
}
