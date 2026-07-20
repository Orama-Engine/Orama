// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Common.Resources.DefaultProvider;
using Orama.Common.Utility;
using Orama.Math;
using SlangShaderSharp;
using System.Collections.Immutable;
using System.Text;
using Veldrith;

namespace Orama.Rendering.Resources;

public sealed class ShaderParameter
{
	/// <summary> The type of the parameter. </summary>
	/// <remarks> Each value should match the relative Slang type. </remarks>
	public enum ParamType
	{
		Int,
		Float,
		Matrix,
		Vector,
		SampledTexture2D
	}

	public string Name { get; }
	public ParamType Type { get; }
	public object? DefaultValue { get; }

	/// <summary> Initializes a new instance of the <see cref="ShaderParameter"/> class. </summary>
	public ShaderParameter(string name, ParamType type, object? defaultValue = null)
	{
		Name = name;
		Type = type;
		DefaultValue = defaultValue;
	}
}

public sealed class ShaderResource
{
	public string Name { get; }
	public ResourceKind Kind { get; }
	public uint Binding { get; }
	public uint Set { get; }
	public uint SizeInBytes { get; }

	public ImmutableArray<ShaderParameter> Parameters { get; }

	public ShaderResource(string name, ResourceKind kind, uint binding, uint set, IEnumerable<ShaderParameter> parameters, uint sizeInBytes)
	{
		Name = name;
		Kind = kind;
		Binding = binding;
		Set = set;
		SizeInBytes = sizeInBytes;

		Parameters = parameters.ToImmutableArray();
	}
}

public class Shader
{
	/// <summary> The name of the shader's pass. </summary>
	public string Pass { get; internal set; } = "None";

	/// <summary> The name of the shader. This is used to import this shader. </summary>
	public string Name { get; internal set; }

	/// <summary> The shader's raw Slang source. </summary>
	/// <remarks> Setting this value will recompile the shader and is a heavy operation. </remarks>
	public string Source
	{
		get => field;
		set
		{
			SlangCompilationResult comp = ShaderBaker.SlangToSpirV(value, Name);
			VertexBytecode = comp.VertexBytes.ToArray();
			FragmentBytecode = comp.FragmentBytes.ToArray();

			foreach (var attribute in comp.ShaderAttributes)
			{
				if (attribute.Name == "ShaderPass")
					Pass = attribute.GetArgumentValueString(0);
			}

			var resources = new List<ShaderResource>();

			foreach (var resource in comp.Resources)
			{
				var parameters = new List<ShaderParameter>();

				for (uint i = 0; i < resource.Type.FieldCount; i++)
				{
					var @field = resource.Type.GetFieldByIndex(i);

					if (!Enum.TryParse(@field.Type.Name, true, out ShaderParameter.ParamType type))
					{
						OramaConsole.Warning($"Unsupported shader parameter type '{@field.Type.Name}'.");
						continue;
					}

					object? defaultValue = null;

					for (uint j = 0; j < @field.AttributeCount; j++)
					{
						var attribute = @field.GetAttribute(j);

						// Hacky
						switch (attribute.Name)
						{
							case "DefaultFloat":
								defaultValue = attribute.GetArgumentValueFloat(0);
								break;

							case "DefaultInt":
								defaultValue = attribute.GetArgumentValueInt(0);
								break;

							case "DefaultFloat2":
								defaultValue = new Vector2(
									attribute.GetArgumentValueFloat(0),
									attribute.GetArgumentValueFloat(1));
								break;

							case "DefaultFloat3":
								defaultValue = new Vector3(
									attribute.GetArgumentValueFloat(0),
									attribute.GetArgumentValueFloat(1),
									attribute.GetArgumentValueFloat(2));
								break;

							case "DefaultFloat4":
								defaultValue = new Vector4(
									attribute.GetArgumentValueFloat(0),
									attribute.GetArgumentValueFloat(1),
									attribute.GetArgumentValueFloat(2),
									attribute.GetArgumentValueFloat(3));
								break;

							case "DefaultTexture":
								defaultValue = Application.ResourceProvider.GetResource<Texture>(
									attribute.GetArgumentValueString(0));
								break;
						}
					}

					parameters.Add(new ShaderParameter(@field.Name, type, defaultValue));
				}

				var elementType = resource.TypeLayout;

				uint size = (uint)elementType.ElementTypeLayout.GetSize(SlangParameterCategory.Uniform);

				resources.Add(new ShaderResource(
					resource.Name,
					ResourceKind.UniformBuffer,
					(uint)resource.GetOffset(SlangParameterCategory.DescriptorTableSlot),
					(uint)resource.GetOffset(SlangParameterCategory.SubElementRegisterSpace),
					parameters,
					size));
			}

			Resources = resources.OrderBy(r => r.Set).ThenBy(r => r.Binding).ToImmutableArray();

			Layouts = Resources
				.GroupBy(r => r.Set)
				.OrderBy(g => g.Key)
				.Select(g => new ResourceLayoutDescription(
					g.OrderBy(r => r.Binding)
					 .Select(r => new ResourceLayoutElementDescription(
						 r.Name,
						 r.Kind,
						 ShaderStages.Vertex | ShaderStages.Fragment))
					 .ToArray()))
				.ToArray();

			field = value;
		}
	}

	/// <summary> The <see cref="IShaderDefaultsProvider"/> used by shaders. </summary>
	/// <remarks> Defaults to <see cref="Rendering.ShaderDefaultsProvider"/>. </remarks>
	public static IShaderDefaultsProvider DefaultsProvider { get; } = new ShaderDefaultsProvider();

	/// <summary> The shader's resource definitions. </summary>
	public ImmutableArray<ShaderResource> Resources { get; private set; } = ImmutableArray<ShaderResource>.Empty;

	/// <summary> The shader's raw SPIR-V bytecode. </summary>
	internal byte[] VertexBytecode { get; private set; } = Array.Empty<byte>();

	/// <summary> The shader's raw SPIR-V bytecode. </summary>
	internal byte[] FragmentBytecode { get; private set; } = Array.Empty<byte>();

	// HACK: This is definitely too close to the GPU
	// We should move this ASAP
	/// <summary> The shader's resource layouts. </summary>
	internal ResourceLayoutDescription[] Layouts = Array.Empty<ResourceLayoutDescription>();

	/// <summary> Initializes a new <see cref="Shader"/> from the specified ShaderLang source. </summary>
	public Shader(string shaderLangSource, string name = "None")
	{
		Name = name;
		Source = shaderLangSource;
	}
}

[ResourceLoader]
internal sealed class ShaderLoader : ResourceLoader<Shader>
{
	/// <inheritdoc/>
	public override Shader? LoadResource(byte[] data, string? name = null) => new(Encoding.UTF8.GetString(data), name ?? "None");
}
