using NeoVeldrid;
using Orama.Common.Resources.DefaultProvider;
using Orama.Common.Utility;
using System.Text;

namespace Orama.Rendering.Resources;

public sealed class ShaderParameter
{
    /// <summary> The type of the parameter. </summary>
    /// <remarks> Each value should match the relative Slang type. </remarks>
    public enum ParamType
    {
        Int,
        Float,
        Float2,
        Float3,
        Float4,
        Matrix4x4
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

// TODO: Should this live in the shader class? Might be too close to the GPU
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
            VertexBytecode = comp.VertexBytes;
            FragmentBytecode = comp.FragmentBytes;

            foreach (var attribute in comp.ShaderAttributes)
                if (attribute.Name == "ShaderPass")
                    Pass = attribute.GetArgumentValueString(0);

            List<ShaderParameter> parameters = new List<ShaderParameter>();

            foreach (var parameter in comp.ShaderParameters)
            {
                ShaderParameter.ParamType type = Enum.Parse<ShaderParameter.ParamType>(parameter.Type.Name, true);
                object? defaultValue = null;
                if (parameter.HasDefaultValue)
                {
                    switch (type)
                    {
                        case ShaderParameter.ParamType.Float:
                            parameter.GetDefaultValueFloat(out float dv);
                            defaultValue = dv;
                            break;
                        case ShaderParameter.ParamType.Int:
                            parameter.GetDefaultValueInt(out long idv);
                            defaultValue = idv;
                            break;
                    }
                }

                parameters.Add(new ShaderParameter(parameter.Name, type, defaultValue));
            }

            Dictionary<string, ShaderResource> resources = new Dictionary<string, ShaderResource>();

            // We use GetOffset because it seems to be the only method to consistently get the correct finalised bindings
            // Be careful around BindingIndex & BindingSpace
            foreach (var resource in comp.Resources)
                resources.Add(resource.Name, new ShaderResource(ResourceKind.UniformBuffer, (uint)resource.GetOffset(SlangShaderSharp.SlangParameterCategory.DescriptorTableSlot), (uint)resource.GetOffset(SlangShaderSharp.SlangParameterCategory.SubElementRegisterSpace)));

            this.parameters = parameters;
            this.resources = resources;

            field = value;
        }
    }

    /// <summary> The shader's raw SPIR-V bytecode. </summary>
    internal byte[] VertexBytecode { get; private set; } = Array.Empty<byte>();

    /// <summary> The shader's raw SPIR-V bytecode. </summary>
    internal byte[] FragmentBytecode { get; private set; } = Array.Empty<byte>();

    /// <summary> The shader's parameter definitions. </summary>
    public IReadOnlyList<ShaderParameter> Parameters => parameters;

    /// <summary> The shader's resource definitions mapped to their names. </summary>
    public IReadOnlyDictionary<string, ShaderResource> Resources => resources;

    private List<ShaderParameter> parameters = new List<ShaderParameter>();
    private Dictionary<string, ShaderResource> resources = new Dictionary<string, ShaderResource>();

    /// <summary> Initializes a new <see cref="Shader"/> from the specified ShaderLang source. </summary>
    public Shader(string shaderLangSource, string name = "None")
    {
        Name = name;
        Source = shaderLangSource;
    }

    // Hack
    public IEnumerable<ResourceLayoutDescription> CreateResourceLayouts()
    {
        return Resources
            .GroupBy(r => r.Value.Set)
            .OrderBy(g => g.Key)
            .Select(g => new ResourceLayoutDescription(
                g.OrderBy(r => r.Value.Binding)
                 .Select(r => new ResourceLayoutElementDescription(
                     r.Key,
                     r.Value.Kind,
                     ShaderStages.Vertex | ShaderStages.Fragment))
                 .ToArray()));
    }
}

[ResourceLoader]
internal class ShaderLoader : ResourceLoader<Shader>
{
    /// <inheritdoc/>
    public override Shader? LoadResource(byte[] data, string? name = null) => new Shader(Encoding.UTF8.GetString(data), name ?? "None");
}