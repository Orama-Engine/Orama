using Orama.Rendering;
using Veldrid;
using Key = Orama.InputManagement.Key;

namespace Orama.InputManagement;

/// <summary>
/// Manages user input.
/// </summary>
public static class Input
{
	private static HashSet<Key> keysDown = new();
	private static HashSet<Key> keysDownLastFrame = new();

	/// <summary> Occurs when a key is pressed. </summary>
	public static event Action<Key>? KeyPressed;

	/// <summary>
	/// Runs each frame
	/// </summary>
	public static void Update()
	{
		if (Window.InternalWindow == null)
			return;

		InputSnapshot snapshot = Window.LatestInputSnapshot;
		
		keysDownLastFrame = new HashSet<Key>(keysDown);
		keysDown.Clear();
		
		foreach (var keyEvent in snapshot.KeyEvents)
			if (keyEvent.Down)
				keysDown.Add(keyEvent.Key.ToEngineKey());

		foreach (var key in keysDown)
			if (!keysDownLastFrame.Contains(key))
				KeyPressed?.Invoke(key);
	}
	
	/// <summary>
	/// Returns true if the specified key is currently down.
	/// </summary>
	/// <param name="key">The key to check.</param>
	public static bool IsKeyDown(Key key) => keysDown.Contains(key);
}