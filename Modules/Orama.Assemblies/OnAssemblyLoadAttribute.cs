using Orama.Common.Utility;
using System.Reflection;

namespace Orama.Assemblies;

/// <summary>
/// Marks a static method to be ran automatically when the assembly is loaded.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OnAssemblyLoadAttribute : Attribute
{
    public static void RunOnAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (method.GetParameters().Length != 0)
                    continue;

                if (method.ReturnType != typeof(void))
                    continue;

                if (!method.IsDefined(typeof(OnAssemblyLoadAttribute), false))
                    continue;

                method.Invoke(null, null);
            }
    }
}
