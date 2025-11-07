using Orama.Rendering;
using Orama.Rendering.Resources;
using Orama.ShaderLang;
using Orama.ShaderLang.Targets;

namespace Orama.Core.Modules.Rendering.Resources;

/// <summary> A wrapper class for a low-level <see cref="Orama.Rendering.Resources.GraphicsShader"/>. </summary>
public class Shader
{
    /// <summary> The name of the shader's pass. </summary>
    public string Pass { get; set; } = "None";

    /// <summary> The underlying <see cref="Orama.Rendering.Resources.GraphicsShader"/> associated with the material. </summary>
    internal GraphicsShader GraphicsShader { get; set; }

    /// <summary> Initializes a new <see cref="Shader"/> from the specified <see cref="Orama.Rendering.Resources.GraphicsShader"/>. </summary>
    internal Shader(GraphicsShader shader) => GraphicsShader = shader;

    /// <summary> Initializes a new <see cref="Shader"/> from the specified vertex and fragment shaders. </summary>
    public Shader(string vertexSource, string fragmentSource) => GraphicsShader = ShaderBaker.GLSLToShader(vertexSource, fragmentSource);

    /// <summary> Initializes a new <see cref="Shader"/> from the specified ShaderLang source. </summary>
    public Shader(string shaderLangSource)
    {
        ShaderLangFormat format = ShaderLangFormat.FromSource(shaderLangSource);
        (string, string) hlsl = HLSLTarget.Compile(format);

        GraphicsShader = ShaderBaker.HLSLToShader(hlsl.Item1, hlsl.Item2);
        Pass = format.Pass ?? "None";
    }
}
