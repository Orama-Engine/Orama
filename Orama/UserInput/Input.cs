using Orama.Rendering;
using Veldrid;

namespace Orama.UserInput;

/// <summary>
/// Access into Input.
/// </summary>
public static class Input
{
	private static HashSet<Key> keysDown = new();
	private static HashSet<Key> keysDownLastFrame = new();

	/// <summary> Called when a key is pressed. </summary>
	public static event Action<Key>? KeyPressed;

	public static void Update()
	{
		if (Window.InternalWindow == null)
			return;

		InputSnapshot snapshot = Window.LatestInputSnapshot;

		keysDownLastFrame = new HashSet<Key>(keysDown);

		// Update key state
		foreach (var keyEvent in snapshot.KeyEvents)
		{
			Key engineKey = keyEvent.Key.ToEngineKey();
			if (keyEvent.Down)
				keysDown.Add(engineKey);
			else
				keysDown.Remove(engineKey);
		}

		// Fire KeyPressed only for newly pressed keys this frame
		foreach (var key in keysDown)
			if (!keysDownLastFrame.Contains(key))
				KeyPressed?.Invoke(key);
	}

	/// <summary>
	/// Checks if a key is currently pressed.
	/// </summary>
	/// <param name="key">Key to check.</param>
	public static bool IsKeyDown(Key key) => keysDown.Contains(key);

	/// <summary>
	/// Checks if a key was pressed this frame.
	/// </summary>
	/// <param name="key">Key to check.</param>
	public static bool IsKeyPressed(Key key) => keysDown.Contains(key) && !keysDownLastFrame.Contains(key);
}
