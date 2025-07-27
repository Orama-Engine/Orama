using Orama.Math;
using System.Xml.Linq;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Orama.Rendering;

public static class Window
{
	public static Sdl2Window InternalWindow { get; internal set; }

	public static InputSnapshot LatestInputSnapshot { get; private set; }

	internal static Action? load;
	internal static Action? update;
	internal static Action? closing;

	public static event Action<bool>? FocusChanged;
	public static event Action<Vector2I>? Resize;

	public static event Action<Vector2I>? Move;
	public static event Action<string[]>? FileDrop;

	public static int Width => InternalWindow.Width;
	public static int Height => InternalWindow.Height;

	/// <summary> The size of the window. </summary>
	public static Vector2I Size
	{
		get => new Vector2I(InternalWindow.Width, InternalWindow.Height);
		set { InternalWindow.Width = value.X; InternalWindow.Height = value.Y; }
	}

	/// <summary> The position of the window. </summary>
	public static Vector2I Position
	{
		get => new Vector2I(InternalWindow.X, InternalWindow.Y);
		set { InternalWindow.X = value.X; InternalWindow.Y = value.Y; }
	}

	/// <summary> The frame rate of the window. </summary>
	public static float FramesPerSecond
	{
		get => InternalWindow.PollIntervalInMs / 1000.0f;
		set { InternalWindow.LimitPollRate = value != 0 && value != double.MaxValue; InternalWindow.PollIntervalInMs = value * 1000.0f; }
	}

	/// <summary> Whether the window is visible. </summary>
	public static bool IsVisible
	{
		get => InternalWindow.Visible;
		set => InternalWindow.Visible = value;
	}

	/// <summary> The state of the window. </summary>
	public static WindowState WindowState
	{
		get => InternalWindow.WindowState;
		set => InternalWindow.WindowState = value;
	}

	public static void Start(string title, Vector2I size, Vector2I position)
	{
		WindowCreateInfo windowInfo = new()
		{
			WindowTitle = title,
			WindowInitialState = WindowState.Normal,
			WindowWidth = size.X,
			WindowHeight = size.Y,
			X = position.X,
			Y = position.Y
		};

		InternalWindow = VeldridStartup.CreateWindow(ref windowInfo);
		LatestInputSnapshot = InternalWindow.PumpEvents();

		load?.Invoke();

		InternalWindow.Resized += () => Resize?.Invoke(Size);
		InternalWindow.Closing += closing;
		InternalWindow.Closed += () => Environment.Exit(0);

		while (InternalWindow.Exists)
		{
			Sdl2Events.ProcessEvents();
			LatestInputSnapshot = InternalWindow.PumpEvents();
			
			update?.Invoke();
		}
	}
}
