
using System.Reflection;

namespace Orama.Utility;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public abstract class InvokableAttribute : Attribute
{
	/// <summary> Invokes all methods marked with this attribute. </summary>
	public static void InvokeAll<T>() where T : InvokableAttribute
	{
		var attrType = typeof(T);

		// Look through all loaded assemblies
		var methods = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			.Where(m => m.IsDefined(attrType, false));

		foreach (var method in methods)
		{
			if (!method.IsStatic)
				throw new InvalidOperationException($"{method.DeclaringType}.{method.Name} must be static to be invoked by {attrType.Name}.");

			method.Invoke(null, null);
		}
	}
}

/// <summary>
/// Marks a static method to be called when the game is initialized.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class OnGameInitializeAttribute : InvokableAttribute { }

/// <summary>
/// Marks a static method to be called when the game is quit.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class OnGameQuitAttribute : InvokableAttribute { }
