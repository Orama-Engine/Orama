
using Orama.Rendering.Native;
using Orama.Rendering.Resources;
using Silk.NET.Core.Native;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from source to compiled shaders.
/// </summary>
public static class ShaderBaker
{
    private static IntPtr compilerInstance;
    private static IntPtr compilerOptions;

    static ShaderBaker()
    {
        ShaderC.InitializeImports();

        compilerInstance = ShaderC.CompilerInitialize();
        compilerOptions = ShaderC.CompileOptionsInitialize();
    }

    /// <summary> Compiles GLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader GLSLToShader(string vertex, string fragment)
    {
        ShaderC.CompileOptionsSetSourceLanguage(compilerOptions, SourceLanguage.GLSL);
        return Compile(vertex, fragment);
    }

    /// <summary> Compiles HLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader HLSLToShader(string vertex, string fragment)
    {
        ShaderC.CompileOptionsSetSourceLanguage(compilerOptions, SourceLanguage.HLSL);
        return Compile(vertex, fragment);
    }

    private static GraphicsShader Compile(string vertex, string fragment)
    {
        IntPtr vertShader = ShaderC.CompileIntoSpv(compilerInstance, vertex, ShaderKind.VertexShader, "vertex.shader", "main", compilerOptions);
        IntPtr fragShader = ShaderC.CompileIntoSpv(compilerInstance, fragment, ShaderKind.FragmentShader, "fragment.shader", "main", compilerOptions);

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
