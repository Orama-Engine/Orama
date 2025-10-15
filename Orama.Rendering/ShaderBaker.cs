using Vortice.ShaderCompiler;
using Orama.Rendering.Resources;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from source to compiled shaders.
/// </summary>
public static class ShaderBaker
{
    private static Vortice.ShaderCompiler.Compiler compiler;

    static ShaderBaker()
    {
        compiler = new();
    }

    /// <summary> Compiles GLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader GLSLToShader(string vertex, string fragment)
    {
        return Compile(vertex, fragment, SourceLanguage.GLSL);
    }

    /// <summary> Compiles HLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader HLSLToShader(string vertex, string fragment)
    {
        return Compile(vertex, fragment, SourceLanguage.HLSL);
    }


    private static GraphicsShader Compile(string vertexSource, string fragmentSource, SourceLanguage sourceLang)
    {
        CompilerOptions options = new();
        options.SourceLanguage = sourceLang;
        options.EntryPoint = "main";

        options.ShaderStage = ShaderKind.VertexShader;
        var vertResult = compiler.Compile(vertexSource, "vertex.shader", options);
        if (vertResult.Status != CompilationStatus.Success)
            throw new Exception("Vertex shader compilation failed: " + vertResult.ErrorMessage);

        options.ShaderStage = ShaderKind.FragmentShader;
        var fragResult = compiler.Compile(fragmentSource, "fragment.shader", options);
        if (fragResult.Status != CompilationStatus.Success)
            throw new Exception("Fragment shader compilation failed: " + fragResult.ErrorMessage);

        GraphicsShader shader = new();
        shader.VertexBytes = vertResult.Bytecode;
        shader.FragmentBytes = fragResult.Bytecode;

        return shader;
    }
}
