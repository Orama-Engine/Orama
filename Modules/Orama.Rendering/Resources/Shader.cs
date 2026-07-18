// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Collections.Immutable;
using System.Text;

using Orama.Common;
using Orama.Common.Resources.DefaultProvider;
using Orama.Common.Utility;
using Orama.Math;

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
	public ResourceKind Kind { get; }
	public uint Binding { get; }
	public uint Set { get; }

	public ShaderResource(ResourceKind kind, uint binding, uint set)
	{
		Kind = kind;
		Binding = binding;
		Set = set;
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

			var parameters = new List<ShaderParameter>();

			foreach (var parameter in comp.ShaderParameters)
			{
				if (!Enum.TryParse(parameter.Type.Name, true, out ShaderParameter.ParamType type))
				{
					OramaConsole.Warning($"Unsupported or invalid shader parameter type '{parameter.Type.Name}'.");
					continue;
				}

				object? defaultValue = null;

				uint attributeCount = parameter.AttributeCount;
				for (uint i = 0; i < attributeCount; i++)
				{
					var attribute = parameter.GetAttribute(i);

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
								attribute.GetArgumentValueFloat(1)
							);
							break;

						case "DefaultFloat3":
							defaultValue = new Vector3(
								attribute.GetArgumentValueFloat(0),
								attribute.GetArgumentValueFloat(1),
								attribute.GetArgumentValueFloat(2)
							);
							break;

						case "DefaultFloat4":
							defaultValue = new Vector4(
								attribute.GetArgumentValueFloat(0),
								attribute.GetArgumentValueFloat(1),
								attribute.GetArgumentValueFloat(2),
								attribute.GetArgumentValueFloat(3)
							);
							break;

						case "DefaultTexture":
							defaultValue = Application.ResourceProvider.GetResource<Texture>(attribute.GetArgumentValueString(0));
							break;

						default:
							continue;
					}

					parameters.Add(new ShaderParameter(parameter.Name, type, defaultValue));
				}

				var resources = new Dictionary<string, ShaderResource>();

				// We use GetOffset because it seems to be the only method to consistently get the correct finalised bindings
				// Be careful around BindingIndex & BindingSpace
				foreach (var resource in comp.Resources)
					resources.Add(resource.Name, new ShaderResource(ResourceKind.UniformBuffer, (uint)resource.GetOffset(SlangShaderSharp.SlangParameterCategory.DescriptorTableSlot), (uint)resource.GetOffset(SlangShaderSharp.SlangParameterCategory.SubElementRegisterSpace)));

				this.Parameters = parameters.ToImmutableArray();
				this.Resources = resources.OrderBy(r => r.Value.Set).ThenBy(r => r.Value.Binding).ToDictionary(r => r.Key, r => r.Value).ToImmutableDictionary();

				this.Layouts = resources
					.GroupBy(r => r.Value.Set)
					.OrderBy(g => g.Key)
					.Select(g => new ResourceLayoutDescription(
						g.OrderBy(r => r.Value.Binding)
						 .Select(r => new ResourceLayoutElementDescription(
							 r.Key,
							 r.Value.Kind,
							 ShaderStages.Vertex | ShaderStages.Fragment))
						 .ToArray()))
					.ToArray();

				field = value;
			}
		}
	}

	/// <summary> The <see cref="IShaderDefaultsProvider"/> used by shaders. </summary>
	/// <remarks> Defaults to <see cref="Rendering.ShaderDefaultsProvider"/>. </remarks>
	public static IShaderDefaultsProvider DefaultsProvider { get; } = new ShaderDefaultsProvider();

	/// <summary> The shader's parameter definitions. </summary>
	public ImmutableArray<ShaderParameter> Parameters { get; private set; }

	/// <summary> The shader's resource definitions mapped to their names. </summary>
	public ImmutableDictionary<string, ShaderResource> Resources { get; private set; } = ImmutableDictionary<string, ShaderResource>.Empty;

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
