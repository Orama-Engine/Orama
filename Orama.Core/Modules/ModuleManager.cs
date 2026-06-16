using Orama.Core.Common;
using Orama.Core.Common.Utility;

namespace Orama.Core.Modules;

/// <summary>
/// Management of the module system.
/// </summary>
public static class ModuleManager
{
    /// <summary> All currently registered modules. </summary>
    public static IReadOnlyDictionary<Type, BaseModule> RegisteredModules => registeredModules;

    private static readonly Dictionary<Type, BaseModule> registeredModules = new Dictionary<Type, BaseModule>();

    /// <summary> Gets a module of type T if it is registered. </summary>
    /// <typeparam name="T"> The type of the module. </typeparam>
    public static T? GetModule<T>() where T : BaseModule => (T?)(registeredModules.TryGetValue(typeof(T), out var module) ? module : null);

    /// <summary> Registers a module of type T. </summary>
    /// <typeparam name="T"> The type of the module. </typeparam>
    public static T RegisterModule<T>() where T : BaseModule, new()
    {
        T module = new T();
        return (T)RegisterModule(module);
    }

    /// <summary> Registers a module. </summary>
    /// <param name="module"> The module to register. </param>
    public static BaseModule RegisterModule(BaseModule module)
    {
        registeredModules.Add(module.GetType(), module);

        Application.OnUpdate += module.Update;
        Application.OnStart += module.Initialize;
        Application.OnExit += module.Dispose;

        var name = module.GetType().Name.ToString();
        EngineConsole.Log($"Registered module: {name}", "ModuleManager");

        return module;
    }

    /// <summary> Unregisters a module of type T. </summary>
    /// <typeparam name="T"> The type of the module. </typeparam>
    public static void UnregisterModule<T>() where T : BaseModule => registeredModules.Remove(typeof(T));
}
