
using System.Reflection;

namespace Orama.Utility;

/// <summary>
/// Marks a static method to be called when the game is initialized.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class OnGameInitializeAttribute : Attribute
{
	/// <summary> Invokes all methods marked with this attribute. </summary>
	public static void InvokeAll()
	{
		// Get all methods marked with this attribute.
		var methods = typeof(OnGameInitializeAttribute).Assembly.GetTypes()
			.SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			.Where(m => m.IsDefined(typeof(OnGameInitializeAttribute), false));

		// Invoke each method.
		foreach (var method in methods)
			method.Invoke(null, null);
	}
}

/// <summary>
/// Marks a static method to be called when the game is quit.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class OnGameQuitAttribute : Attribute
{
	/// <summary> Invokes all methods marked with this attribute. </summary>
	public static void InvokeAll()
	{
		// Get all methods marked with this attribute.
		var methods = typeof(OnGameQuitAttribute).Assembly.GetTypes()
			.SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			.Where(m => m.IsDefined(typeof(OnGameQuitAttribute), false));

		// Invoke each method.
		foreach (var method in methods)
			method.Invoke(null, null);
	}
}
