// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Reflection;
using System.Runtime.Loader;

namespace Orama.Assemblies;

/// <summary>
/// Unloadable <see cref="System.Reflection.Assembly"/> with cached reflection.
/// </summary>
public sealed class OramaAssembly
{
	/// <summary> The path of the <see cref="OramaAssembly"/> </summary>
	public string Path { get; }

	/// <summary> The internal <see cref="System.Reflection.Assembly"/> instance loaded into memory. </summary>
	public Assembly Assembly { get; }

	/// <summary> The load context of the <see cref="OramaAssembly"/> or null if required assembly. </summary>
	internal AssemblyLoadContext? LoadContext { get; }

	/// <summary> Invoked when the <see cref="OramaAssembly"/> is <b>successfully</b> unloaded. </summary>
	public event Action<OramaAssembly> Unloaded = delegate { };

	/// <summary> All <see cref="Type"/>s in the <see cref="OramaAssembly"/>. </summary>
	/// <remarks> This is a cached property. </remarks>
	public Type[] Types
	{
		get
		{
			if (field is not null)
				return field;

			field = Assembly.GetTypes();
			return field;
		}
	}

	/// <summary> All static methods in the <see cref="OramaAssembly"/> marked with <see cref="OnAssemblyLoadAttribute"/>. </summary>
	/// <remarks> This is a cached property. </remarks>
	public Action[] AssemblyLoadDelegates
	{
		get
		{
			if (field is not null)
				return field;

			List<Action> delegates = new();

			foreach (Type type in Types)
			{
				foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (method.GetParameters().Length != 0)
						continue;

					if (method.ReturnType != typeof(void))
						continue;

					if (!method.CustomAttributes.Any(a => a.AttributeType == typeof(OnAssemblyLoadAttribute)))
						continue;

					Delegate del = method.CreateDelegate(typeof(Action));
					delegates.Add((Action)del);
				}
			}

			field = delegates.ToArray();
			return field;
		}
	}

	/// <summary> Initializes a new instance of <see cref="OramaAssembly"/>. </summary>
	internal OramaAssembly(string path, AssemblyLoadContext? loadContext, Assembly assembly)
	{
		Path = path;
		LoadContext = loadContext;
		Assembly = assembly;
	}

	/// <summary> Tries to unload the <see cref="OramaAssembly"/>. </summary>
	/// <returns> <see langword="true"/> if the <see cref="OramaAssembly"/> was unloaded, otherwise <see langword="false"/>. </returns>
	public bool TryUnload()
	{
		if (LoadContext is null || LoadContext.IsCollectible == false)
			return false;

		LoadContext.Unload();
		Unloaded(this);
		return true;
	}

	/// <inheritdoc/>
	public override string ToString() => $"{Path} ({LoadContext})";

	public static implicit operator Assembly(OramaAssembly asm) => asm.Assembly;
	public static implicit operator OramaAssembly(Assembly asm) => new(asm.Location, AssemblyLoadContext.GetLoadContext(asm), asm);
}
