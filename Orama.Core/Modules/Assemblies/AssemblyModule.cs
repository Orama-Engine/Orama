
namespace Orama.Core.Modules.Assemblies;

/// <summary>
/// Module responsible for management of external assemblies.
/// </summary>
public class AssemblyModule : BaseModule
{
    private readonly HashSet<ExternalAssembly> _assemblies = new();

    /// <summary> Called when an assembly is loaded. </summary>
    public Action<ExternalAssembly>? AssemblyLoaded { get; set; }

    /// <summary> Called when an assembly is unloaded. </summary>
    public Action<ExternalAssembly>? AssemblyUnloaded { get; set; }

    /// <summary> The assemblies loaded into the process. </summary>
    public IReadOnlyCollection<ExternalAssembly> Assemblies => _assemblies;

    /// <inheritdoc/>
    public override void Dispose() => UnloadAll();

    /// <summary> Loads an assembly from the specified path. </summary>
    public ExternalAssembly LoadFromPath(string path)
    {
        string absolutePath = System.IO.Path.GetFullPath(path);

        var context = new ExternalAssemblyLoadContext();
        var assembly = context.LoadFromAssemblyPath(absolutePath);

        return new ExternalAssembly(absolutePath, context, assembly);
    }

    /// <summary> Unloads the specified assembly. </summary>
    public void Unload(ExternalAssembly assembly)
    {
        if (_assemblies.Remove(assembly))
        {
            assembly.LoadContext.Unload();
            AssemblyUnloaded?.Invoke(assembly);
        }
    }

    /// <summary> Unloads all currently loaded assemblies. </summary>
    public void UnloadAll()
    {
        foreach (var asm in _assemblies)
        {
            asm.LoadContext.Unload();
            AssemblyUnloaded?.Invoke(asm);
        }

        _assemblies.Clear();
    }
}
