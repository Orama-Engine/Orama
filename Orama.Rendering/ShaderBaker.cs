
using Orama.Rendering.Native;
using Orama.Rendering.Resources;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from source to compiled shaders.
/// </summary>
public static class ShaderBaker
{
    private static IntPtr compilerInstance;

    static ShaderBaker()
    {
        ShaderC.InitializeImports();
        compilerInstance = ShaderC.CompilerInitialize();
    }

    /// <summary> Compiles GLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader GLSLToShader(string vertex, string fragment)
    {
        Console.WriteLine(compilerInstance);
        return new();
    }

    /// <summary> Compiles HLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader HLSLToShader(string vertex, string fragment) => new();
}
