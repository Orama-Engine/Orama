using System.Runtime.Loader;

namespace Orama.Utility;

/// <summary>
/// An assembly load context for Orama game assemblies.
/// </summary>
internal class OramaAssemblyLoadContext : AssemblyLoadContext
{
	public OramaAssemblyLoadContext() : base(isCollectible: true) { }
}
