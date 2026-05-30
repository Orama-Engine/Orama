using Orama.ShaderLang;
using Orama.ShaderLang.Targets;

namespace Orama.Core.Modules.Rendering.Resources;

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
    public string Pass { get; private set; } = "None";

    /// <summary> The shader's raw ShaderLang source. </summary>
    public string Source
    {
        get => field;
        set
        {
            ShaderLangFormat format = ShaderLangFormat.FromSource(value);
            (string, string) hlsl = HLSLTarget.Compile(format);
            Vertex = hlsl.Item1;
            Fragment = hlsl.Item2;

            Pass = format.Pass ?? "None";
            
            field = value;
        }
    }

    /// <summary> The shader's raw HLSL vertex shader source. </summary>
    internal string Vertex { get; private set; } = string.Empty;

    /// <summary> The shader's raw HLSL fragment shader source. </summary>
    internal string Fragment { get; private set; } = string.Empty;

    /// <summary> The shader's parameter definitions. </summary>
    public IReadOnlyList<ShaderParameter> Parameters => parameters;

    private readonly List<ShaderParameter> parameters = new List<ShaderParameter>();

    /// <summary> Initializes a new <see cref="Shader"/> from the specified ShaderLang source. </summary>
    public Shader(string shaderLangSource) => Source = shaderLangSource;
}
