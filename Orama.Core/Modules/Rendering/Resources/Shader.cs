using Orama.ShaderLang;
using Orama.ShaderLang.Targets;

namespace Orama.Core.Modules.Rendering.Resources;

public class Shader
{
    /// <summary> The name of the shader's pass. </summary>
    public string Pass { get; }

    /// <summary> The shader's raw HLSL vertex shader source. </summary>
    internal string Vertex {  get; }

    /// <summary> The shader's raw HLSL fragment shader source. </summary>
    internal string Fragment { get; }

    /// <summary> The shader's parameters. </summary>
    public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

    /// <summary> Initializes a new <see cref="Shader"/> from the specified ShaderLang source. </summary>
    public Shader(string shaderLangSource)
    {
        ShaderLangFormat format = ShaderLangFormat.FromSource(shaderLangSource);
        (string, string) hlsl = HLSLTarget.Compile(format);

        Vertex = hlsl.Item1;
        Fragment = hlsl.Item2;

        Pass = format.Pass ?? "None";
    }
}
