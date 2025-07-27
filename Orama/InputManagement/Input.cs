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

	/// <summary>
	/// Runs each frame
	/// </summary>
	public static void Update()
	{
		if (Window.InternalWindow == null)
		{
			return;
		}

		InputSnapshot snapshot = Window.LatestInputSnapshot;
		
		keysDownLastFrame = new HashSet<Key>(keysDown);
		keysDown.Clear();
		
		foreach (var keyEvent in snapshot.KeyEvents)
		{
			if (keyEvent.Down)
			{
				keysDown.Add(keyEvent.Key.ToEngineKey());
			}
		}

		foreach (var key in keysDown)
		{
			if (!keysDownLastFrame.Contains(key))
			{
				KeyPressed?.Invoke(key);
			}
		}
	}
	
	public static bool IsKeyDown(Key key) => keysDown.Contains(key);
	
	public static event Action<Key>? KeyPressed;
}