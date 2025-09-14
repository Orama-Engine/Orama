namespace Orama.Modules;

/// <summary>
/// Provides management of all <see cref="Module"/>s.
/// </summary>
public static class ModuleManager
{
	private static Dictionary<Type, Module> modules = new();

	/// <summary>
	/// Register a new <see cref="Module"/> of type <typeparamref name="T"/> if it isn't already registered.
	/// Returns an existing instance if one already exists.
	/// </summary>
	/// <typeparam name="T">The type of <see cref="Module"/> to register.</typeparam>
	/// <returns>The registered or existing instance of <typeparamref name="T"/>.</returns>
	public static T RegisterModule<T>() where T : Module, new()
	{
		var type = typeof(T);
		if (modules.TryGetValue(type, out var existing))
			return (T)existing;

		var instance = new T();
		modules[type] = instance;
		return instance;
	}
	
	/// <summary>
	/// Unregister an existing <see cref="Module"/> of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of <see cref="Module"/> to unregister.</typeparam>
	public static void UnregisterModule<T>() where T : Module
	{
		modules.Remove(typeof(T));
	}

	/// <summary>
	/// Retrieve a registered <see cref="Module"/> by type.
	/// </summary>
	/// <typeparam name="T">The type of <see cref="Module"/> to grab.</typeparam>
	/// <returns>Grabbed <see cref="Module"/>.</returns>
	public static T? GetModule<T>() where T : Module
	{
		modules.TryGetValue(typeof(T), out var module);
		return (T?)module;
	}
	
	/// <summary>
	/// Calls <see cref="Module.Start"/> on all enabled modules.
	/// </summary>
	public static void Start()
	{
		foreach (var module in modules.Values)
			if (module.Enabled)
				module.Start();
	}
	
	/// <summary>
	/// Calls <see cref="Module.Update"/> on all enabled modules.
	/// </summary>
	public static void Update()
	{
		foreach (var module in modules.Values)
			if (module.Enabled)
				module.Update();
	}
}