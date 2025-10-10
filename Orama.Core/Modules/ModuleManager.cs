
using Microsoft.Extensions.DependencyModel;
using Orama.Core.Common;
using System.Reflection;

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
    /// <remarks> If you need a non nullable return value, use <see cref="GetOrRegisterModule{T}"/> instead. </remarks>
    public static T? GetModule<T>() where T : BaseModule => (T?)(registeredModules.TryGetValue(typeof(T), out var module) ? module : null);

    /// <summary> Gets a module of type T or registers it if it is not registered. </summary>
    /// <typeparam name="T"> The type of the module. </typeparam>
    public static T GetOrRegisterModule<T>() where T : BaseModule, new()
    {
        if (GetModule<T>() is T module)
            return module;

        return RegisterModule<T>();
    }

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
        foreach (var dependency in module.Dependencies)
        {
            if (!registeredModules.ContainsKey(dependency))
                RegisterModule(Activator.CreateInstance(dependency) as BaseModule ?? throw new ArgumentException($"Could not create dependency module {dependency.Name}"));
        }

        registeredModules.Add(module.GetType(), module);

        Application.OnUpdate += module.Update;
        Application.OnStart += module.Initialize;
        Application.OnExit += module.Dispose;

        return module;
    }

    /// <summary> Unregisters a module of type T. </summary>
    /// <typeparam name="T"> The type of the module. </typeparam>
    public static void UnregisterModule<T>() where T : BaseModule => registeredModules.Remove(typeof(T));
}
