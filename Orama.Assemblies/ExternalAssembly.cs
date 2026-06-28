using System.Reflection;

namespace Orama.Assemblies;

/// <summary>
/// An assembly that is loaded from an external source.
/// </summary>
public sealed class ExternalAssembly
{
    /// <summary> The path of the <see cref="ExternalAssembly"/> </summary>
    public string Path { get; }

    /// <summary> The actual <see cref="System.Reflection.Assembly"/> instance loaded into memory. </summary>
    public Assembly Assembly { get; }

    /// <summary> The load context of the <see cref="ExternalAssembly"/> </summary>
    internal ExternalAssemblyLoadContext LoadContext { get; }

    /// <summary> Initializes a new instance of <see cref="ExternalAssembly"/>. </summary>
    internal ExternalAssembly(string path, ExternalAssemblyLoadContext loadContext, Assembly assembly)
    {
        Path = path;
        LoadContext = loadContext;
        Assembly = assembly;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Path} ({LoadContext})";
}
