using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering.Resources;

/// <summary> A wrapper class for a low-level <see cref="Orama.Rendering.Resources.GraphicsShader"/>. </summary>
public class Shader
{
    /// <summary> The underlying <see cref="Orama.Rendering.Resources.GraphicsShader"/> associated with the material. </summary>
    internal GraphicsShader GraphicsShader { get; set; }

    /// <summary> Initializes a new <see cref="Shader"/> from the specified <see cref="Orama.Rendering.Resources.GraphicsShader"/>. </summary>
    internal Shader(GraphicsShader shader) => GraphicsShader = shader;

    /// <summary> Initializes a new <see cref="Shader"/> from the specified vertex and fragment shaders. </summary>
    public Shader(string vertexSource, string fragmentSource) => GraphicsShader = ShaderBaker.GLSLToShader(vertexSource, fragmentSource);
}
