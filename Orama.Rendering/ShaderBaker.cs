
using Orama.Rendering.Resources;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from source to compiled shaders.
/// </summary>
public static class ShaderBaker
{
    /// <summary> Compiles GLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader GLSLToShader(string source) => new();

    /// <summary> Compiles HLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader HLSLToShader(string source) => new();
}
