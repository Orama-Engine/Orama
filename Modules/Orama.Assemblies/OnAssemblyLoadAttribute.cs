// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Reflection;

namespace Orama.Assemblies;

/// <summary>
/// Marks a static method to be ran automatically when the assembly is loaded.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OnAssemblyLoadAttribute : Attribute
{
	public static void RunOnAssembly(OramaAssembly assembly)
	{
		foreach (var type in assembly.Types)
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
