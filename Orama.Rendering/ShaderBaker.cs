using Orama.Rendering.Native;
using Orama.Rendering.Resources;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from source to compiled shaders.
/// </summary>
public static class ShaderBaker
{
    private static IntPtr shaderCCompiler;
    private static IntPtr shaderCCompilerOptions;

    static ShaderBaker()
    {
        ShaderC.InitializeImports();

        shaderCCompiler = ShaderC.CompilerInitialize();
        shaderCCompilerOptions = ShaderC.CompileOptionsInitialize();
    }

    /// <summary> Compiles GLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader GLSLToShader(string vertex, string fragment)
    {
        ShaderC.CompileOptionsSetSourceLanguage(shaderCCompilerOptions, SourceLanguage.GLSL);
        return Compile(vertex, fragment);
    }

    /// <summary> Compiles HLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader HLSLToShader(string vertex, string fragment)
    {
        ShaderC.CompileOptionsSetSourceLanguage(shaderCCompilerOptions, SourceLanguage.HLSL);
        return Compile(vertex, fragment);
    }

    private static GraphicsShader Compile(string vertex, string fragment)
    {
        IntPtr vertShader = ShaderC.CompileIntoSpv(shaderCCompiler, vertex, ShaderKind.VertexShader, "vertex.shader", "main", shaderCCompilerOptions);
        IntPtr fragShader = ShaderC.CompileIntoSpv(shaderCCompiler, fragment, ShaderKind.FragmentShader, "fragment.shader", "main", shaderCCompilerOptions);

        if (ShaderC.ResultGetCompilationStatus(vertShader) != 0 || ShaderC.ResultGetCompilationStatus(fragShader) != 0)
        {
            throw new Exception("Compilation failed: " + ShaderC.ResultGetErrorMessage(vertShader) + " " + ShaderC.ResultGetErrorMessage(fragShader));
        }
        else
        {
            GraphicsShader output = new();

            output.VertexBytes = ShaderC.ResultGetBytes(vertShader);
            output.FragmentBytes = ShaderC.ResultGetBytes(fragShader);

            return output;
        }
    }
}
