using System.Reflection;
using System.Runtime.Loader;

namespace Orama.Assemblies;

internal class ExternalAssemblyLoadContext : AssemblyLoadContext
{
    public ExternalAssemblyLoadContext() : base(isCollectible: true) { }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        return null;    // Use the default load behavior
    }
}
