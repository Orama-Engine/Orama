using System.Reflection;

namespace Orama.Utility;

/// <summary>
/// Manages loading, unloading and hot-reloading .NET assemblies.
/// </summary>
public static class AssemblyLoader
{
	// This many load contexts is probably very inefficient
	private static readonly Dictionary<string, (Assembly Assembly, OramaAssemblyLoadContext Context)> assemblies = new();

	/// <summary>
	/// Loads the assembly at the specified path.
	/// </summary>
	/// <param name="assemblyPath">Path to the assembly to load.</param>
	public static Assembly LoadAssembly(string assemblyPath)
	{
		var context = new OramaAssemblyLoadContext();

		// Prevent file locking issues by reading the file into memory
		byte[] raw = File.ReadAllBytes(assemblyPath);
		using var ms = new MemoryStream(raw);

		var assembly = context.LoadFromStream(ms);
		assemblies[assemblyPath] = (assembly, context);

		return assembly;
	}

	/// <summary>
	/// Unloads the assembly at the specified path.
	/// </summary>
	/// <param name="assemblyPath">Path to the assembly to unload.</param>
	public static void UnloadAssembly(string assemblyPath)
	{
		if (assemblies.TryGetValue(assemblyPath, out var entry))
		{
			assemblies.Remove(assemblyPath);
			entry.Context.Unload();

			// Run GC
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}

	#region Reflection Methods

	/// <summary>
	/// Gets the type with the specified name from loaded assemblies.
	/// </summary>
	/// <param name="typeName">Name of the type to get.</param>
	/// <returns>The type with the specified name if found, otherwise null.</returns>
	public static Type? GetType(string typeName)
	{
		foreach (var assembly in assemblies.Values)
		{
			var type = assembly.Assembly.GetType(typeName);
			if (type != null)
				return type;
		}

		return null;
	}

	#endregion
}

