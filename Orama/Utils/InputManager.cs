using Orama.Rendering;
using Veldrid;
using Key = Orama.Resources.Key;

namespace Orama.Utils;

/// <summary>
/// Manages user input.
/// </summary>
public static class InputManager
{
	private static HashSet<Key> keysDown = new();

	/// <summary>
	/// Runs each frame
	/// </summary>
	/// <param name="deltaTime"></param>
	public static void Update(float deltaTime)
	{
		if (Window.InternalWindow == null)
		{
			return;
		}
		
		InputSnapshot snapshot = Window.InternalWindow.PumpEvents();
		
		keysDown.Clear();
		foreach (var keyEvent in snapshot.KeyEvents)
		{
			if (keyEvent.Down)
			{
				keysDown.Add(keyEvent.Key.ToEngineKey());
				Console.WriteLine($"Key pressed: {keyEvent.Key}");
			}
		}
	}
	
	public static bool IsKeyDown(Key key) => keysDown.Contains(key);
}