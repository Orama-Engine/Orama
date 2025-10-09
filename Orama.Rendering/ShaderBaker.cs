
using Orama.Rendering.Resources;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from source to Spir-V.
/// </summary>
public static class ShaderBaker
{
    /// <summary> Converts GLSL source to Spir-V. </summary>
    public static GraphicsShader GLSLToSpirV(string source) => new();

    /// <summary> Converts HLSL source to Spir-V. </summary>
    public static GraphicsShader HLSLToSpirV(string source) => new();
}
