using Orama.Common.Resources.DefaultProvider;
using Orama.Rendering;
using SlangShaderSharp;
using System.Text;

namespace Orama.Rendering.Resources;

public sealed class ShaderParameter
{
    public enum ParamType
    {
        Float,
        Float2,
        Float3,
        Float4,
        Matrix4x4
    }

    public string Name { get; }
    public ParamType Type { get; }
    public uint Binding { get; }

    /// <summary> Initializes a new instance of the <see cref="ShaderParameter"/> class. </summary>
    public ShaderParameter(string name, ParamType type, uint binding)
    {
        Name = name;
        Type = type;
        Binding = binding;
    }
}

public class Shader
{
    /// <summary> The name of the shader's pass. </summary>
    public string Pass { get; private set; }

    /// <summary> The name of the shader. This is used to import this shader. </summary>
    public string Name { get; private set; }

    /// <summary> The shader's raw ShaderLang source. </summary>
    public string Source
    {
        get => field;
        set
        {
            (byte[] Vert, byte[] Frag) spirv = ShaderBaker.SlangToSpirV(value, Name);
            VertexBytecode = spirv.Item1;
            FragmentBytecode = spirv.Item2;
            
            field = value;
        }
    }

    /// <summary> The shader's raw SPIR-V bytecode. </summary>
    internal byte[] VertexBytecode { get; private set; } = Array.Empty<byte>();

    /// <summary> The shader's raw SPIR-V bytecode. </summary>
    internal byte[] FragmentBytecode { get; private set; } = Array.Empty<byte>();

    /// <summary> The shader's parameter definitions. </summary>
    public IReadOnlyList<ShaderParameter> Parameters => parameters;

    private readonly List<ShaderParameter> parameters = new List<ShaderParameter>();

    /// <summary> Initializes a new <see cref="Shader"/> from the specified ShaderLang source. </summary>
    public Shader(string shaderLangSource, string name = "None", string pass = "None")
    {
        Name = name;
        Source = shaderLangSource;
        Pass = pass;
    }
}

[ResourceLoader]
internal class ShaderLoader : ResourceLoader<Shader>
{
    /// <inheritdoc/>
    public override Shader? LoadResource(byte[] data, string? name = null) => new Shader(Encoding.UTF8.GetString(data), name ?? "None");
}