
using Orama.Core.Common.Utility;

namespace Orama.Modules;

/// <summary>
/// Orchestrates all <see cref="BaseModule"/>s.
/// </summary>
public static class ModuleManager
{
    /// <summary> All registered <see cref="BaseModule"/>s. </summary>
    public static IReadOnlyList<BaseModule> Modules => modules.Values.ToList();

    private static Dictionary<Type, BaseModule> modules = new();

    /// <summary> Registers a new <see cref="BaseModule"/> of type <typeparamref name="T"/>. If this is called after <see cref="InitializeAll"/> the <see cref="BaseModule"/> will need to manually call <see cref="BaseModule.Initialize"/>.</summary>
    /// <returns> The registered <see cref="BaseModule"/> instance. </returns>
    public static T RegisterModule<T>() where T : BaseModule
    {
        var module = Activator.CreateInstance<T>();
        modules.Add(typeof(T), module);
        return module;
    }

    /// <summary> Gets a registered <see cref="BaseModule"/> of type <typeparamref name="T"/>. </summary>
    /// <returns> The registered <see cref="BaseModule"/> instance or null if not found. </returns>
    public static T? GetModule<T>() where T : BaseModule => (T?)modules[typeof(T)];

    /// <summary> Unregisters a <see cref="BaseModule"/>. </summary>
    internal static void UnregisterModule(Type type) => modules.Remove(type);

    /// <summary> Initializes all <see cref="BaseModule"/>s currently registered. </summary>
    public static void InitializeAll()
    {
        foreach (var module in modules.Values)
        {
            module.Initialize();
            module.IsInitialized = true;

            EngineConsole.Log($"Initialized module {module.GetType().Name}", "ModuleManager");
        }
    }

    /// <summary> Disposes all <see cref="BaseModule"/>s currently registered. </summary>
    public static void DisposeAll()
    {
        foreach (var module in modules.Values)
        {
            module.IsInitialized = false;
            module.Dispose();

            EngineConsole.Log($"Disposed module {module.GetType().Name}", "ModuleManager");
        }
    }
}
