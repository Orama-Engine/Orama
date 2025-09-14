namespace Orama.Modules;

/// <summary>
/// Manages all <see cref="Module"/>s
/// </summary>
public static class ModuleManager
{
	private static List<Module> modules = new();

	/// <summary>
	/// Register a <see cref="Module"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static T RegisterModule<T>() where T : Module, new()
	{
		var existing = modules.OfType<T>().FirstOrDefault();
		if (existing != null)
			return existing;

		var created = new T();
		modules.Add(created);
		return created;
	}
	
	/// <summary>
	/// Unregister a <see cref="Module"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static void UnregisterModule<T>() where T : Module
	{
		var existing = modules.OfType<T>().FirstOrDefault();
		if (existing != null)
			modules.Remove(existing);
	}
	
	/// <summary>
	/// Get a <see cref="Module"/> by type or name.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static T? GetModule<T>() where T : Module
		=> modules.OfType<T>().FirstOrDefault();
	
	// Run Module events
	public static void Start()
	{
		foreach (var module in modules.Where(module => module.Enabled))
			module.Start();
	}
	public static void Update()
	{
		foreach (var module in modules.Where(module => module.Enabled))
			module.Update();
	}
}