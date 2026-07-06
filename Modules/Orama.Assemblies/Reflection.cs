using System.Reflection;

namespace Orama.Assemblies;

/// <summary>
/// Orama-Optimized .NET Reflection.
/// </summary>
public static class Reflection
{
    /// <summary> All loaded <see cref="Assembly"/>s in the current <see cref="AppDomain"/> that belong to the engine (non-system). </summary>
    public static List<OramaAssembly> GameAssemblies { get; } = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.FullName?.Contains("System") ?? false).Select(x => (OramaAssembly)x).ToList();
}
