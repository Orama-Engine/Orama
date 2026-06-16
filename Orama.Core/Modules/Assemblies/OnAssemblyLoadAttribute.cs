using System.Reflection;

namespace Orama.Core.Modules.Assemblies;

/// <summary>
/// Marks a static method to be ran automatically when the assembly is loaded.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OnAssemblyLoadAttribute : Attribute
{
    public static void RunLoadAttributes(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                if (method.IsDefined(typeof(OnAssemblyLoadAttribute), false) && method.GetParameters().Length == 0)
                    method.Invoke(null, null);
    }
}
