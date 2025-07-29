using System.Numerics;
using Orama.Rendering;
using Veldrid.Sdl2;
using Veldrid;

namespace Orama.UserInput;

/// <summary>
/// Access into Input.
/// </summary>
public static class Input
{
	/// <summary> Whether the cursor is visible. </summary>
	public static bool CursorVisible { get; set; } = true;

	/// <summary> Whether the cursor is locked to the center of the window. </summary>
	public static bool CursorLocked { get; set; } = false;

	/// <summary> The current position of the mouse. </summary>
	public static Vector2 MousePosition { get; private set; }

	/// <summary> The change in mouse position since the last frame. </summary>
	public static Vector2 MouseDelta { get; private set; }

	private static HashSet<Key> keysDown = new();
	private static HashSet<MouseButton> buttonsDown = new();
	private static Vector2 mousePositionLastFrame = new();
	private static HashSet<Key> keysDownLastFrame = new();
	private static HashSet<MouseButton> buttonsDownLastFrame = new();

	public static void Update()
	{
		if (Window.InternalWindow == null)
			return;

		// Update cursor state
		// TODO: Non-native way to do this?
		Sdl2Native.SDL_ShowCursor(CursorVisible ? 1 : 0);
		if (CursorLocked)
			Sdl2Native.SDL_WarpMouseInWindow(Window.InternalWindow.SdlWindowHandle, Window.Width / 2, Window.Height / 2);

		InputSnapshot snapshot = Window.LatestInputSnapshot;

		// Update mouse position
		MousePosition = new Vector2(snapshot.MousePosition.X, snapshot.MousePosition.Y);
		
		// If cursor is locked, mouse delta is distance from window center to avoid snapping back
		if (CursorLocked)
			MouseDelta = MousePosition - new Vector2(Window.Width / 2, Window.Height / 2);
		else
			MouseDelta = MousePosition - mousePositionLastFrame;

		mousePositionLastFrame = MousePosition;
		keysDownLastFrame = new HashSet<Key>(keysDown);
		buttonsDownLastFrame = new HashSet<MouseButton>(buttonsDown);

		// Update key state
		foreach (var keyEvent in snapshot.KeyEvents)
		{
			Key engineKey = keyEvent.Key.ToEngineKey();
			if (keyEvent.Down)
				keysDown.Add(engineKey);
			else
				keysDown.Remove(engineKey);
		}
		
		// Update Mouse Button state
		foreach (var mouseButtonEvent in snapshot.MouseEvents)
		{
			MouseButton engineMouseButton = mouseButtonEvent.MouseButton.ToEngineMouseButton();
			if (mouseButtonEvent.Down)
				buttonsDown.Add(engineMouseButton);
			else
				buttonsDown.Remove(engineMouseButton);
		}
	}

	/// <summary>
	/// Checks if a key is currently pressed.
	/// </summary>
	/// <param name="key">Key to check.</param>
	public static bool IsKeyDown(Key key) => keysDown.Contains(key);
	
	/// <summary>
	/// Checks if a mouse button is currently pressed.
	/// </summary>
	/// <param name="button"></param>
	/// <returns></returns>
	public static bool IsMouseButtonDown(MouseButton button) => buttonsDown.Contains(button);

	/// <summary>
	/// Checks if a key was pressed this frame.
	/// </summary>
	/// <param name="key">Key to check.</param>
	public static bool KeyPressed(Key key) => keysDown.Contains(key) && !keysDownLastFrame.Contains(key);
	
	/// <summary>
	/// Checks if a mouse button was pressed this frame.
	/// </summary>
	/// <param name="mouseButton"></param>
	/// <returns></returns>
	public static bool MouseButtonPressed(MouseButton mouseButton) => buttonsDown.Contains(mouseButton) && !buttonsDownLastFrame.Contains(mouseButton);
}
