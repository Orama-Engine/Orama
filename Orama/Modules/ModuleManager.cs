namespace Orama.Modules;

/// <summary>
/// Manages all <see cref="Module"/>s
/// </summary>
public static class ModuleManager
{
	private static List<Module> modules = new();
	
	/// <summary>
	/// Register a <see cref="Module"/>
	/// </summary>
	/// <param name="module"></param>
	public static void RegisterModule(Module module) => modules.Add(module);
	
	/// <summary>
	/// Deregister a <see cref="Module"/>
	/// </summary>
	/// <param name="module"></param>
	public static void UnregisterModule(Module module) => modules.Remove(module);
	
	/// <summary>
	/// Get a <see cref="Module"/> by type or name.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static T? GetModule<T>() where T : Module
		=> modules.OfType<T>().FirstOrDefault();
	
	// Run Module events
	public static void Start() => modules.ForEach(module => module.Start());
	public static void Update()
	{
		foreach (var module in modules.Where(module => module.Enabled))
			module.Update();
	}
}