using System.Collections.Concurrent;
using Vortice.ShaderCompiler;

namespace Orama.Rendering;

/// <summary>
/// Bakes GLSL and HLSL shaders to SPIRV.
/// </summary>
public static class ShaderBaker
{
    private static readonly Compiler compiler = new();

    /// <summary> Compiles GLSL source to SPIRV. </summary>
    public static (byte[] Vert, byte[] Frag) GLSLToShader(string vertex, string fragment) => Compile(vertex, fragment, SourceLanguage.GLSL);

    /// <summary> Compiles HLSL source to SPIRV. </summary>
    public static (byte[] Vert, byte[] Frag) HLSLToShader(string vertex, string fragment) => Compile(vertex, fragment, SourceLanguage.HLSL);

    private static (byte[] Vert, byte[] Frag) Compile(string vertexSource, string fragmentSource, SourceLanguage sourceLang)
    {
        CompilerOptions opts = new()
        {
            SourceLanguage = sourceLang,
            EntryPoint = "main"
        };

        opts.ShaderStage = ShaderKind.VertexShader;
        var vert = compiler.Compile(vertexSource, "vertex.shader", opts);
        if (vert.Status != CompilationStatus.Success)
            throw new Exception("Vertex shader compilation failed: " + vert.ErrorMessage);

        opts.ShaderStage = ShaderKind.FragmentShader;
        var frag = compiler.Compile(fragmentSource, "fragment.shader", opts);
        if (frag.Status != CompilationStatus.Success)
            throw new Exception("Fragment shader compilation failed: " + frag.ErrorMessage);

        return (vert.Bytecode, frag.Bytecode);
    }
}
