// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Assemblies;

/// <summary>
/// Marks a static method to be ran automatically when the assembly is loaded.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OnAssemblyLoadAttribute : Attribute
{
	public static void RunOnAssembly(OramaAssembly assembly)
	{
		foreach (Action del in assembly.AssemblyLoadDelegates)
			del();
	}
}
