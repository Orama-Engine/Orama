// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Runtime.Loader;

using Orama.Common;

namespace Orama.Assemblies;

/// <summary>
/// Module responsible for management of external assemblies.
/// </summary>
public class AssemblyModule : BaseModule
{
	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		UnloadAll();
	}

	/// <inheritdoc/>
	public override void Initialize()
	{
		foreach (var loadedAssembly in Reflection.GameAssemblies)
			OnAssemblyLoadAttribute.RunOnAssembly(loadedAssembly);
	}

	/// <summary> Loads an assembly from the specified path. </summary>
	public OramaAssembly LoadFromPath(string path)
	{
		string absolutePath = System.IO.Path.GetFullPath(path);

		var context = new AssemblyLoadContext("OramaPlugin", true);
		var assembly = context.LoadFromAssemblyPath(absolutePath);

		OramaAssembly asm = new(absolutePath, context, assembly);
		Reflection.GameAssemblies.Add(asm);
		asm.Unloaded += (a) => Reflection.GameAssemblies.Remove(a);

		OnAssemblyLoadAttribute.RunOnAssembly(assembly);

		return asm;
	}

	/// <summary> Unloads all currently loaded assemblies. </summary>
	public void UnloadAll()
	{
		foreach (var asm in Reflection.GameAssemblies)
			asm.TryUnload();
	}
}
