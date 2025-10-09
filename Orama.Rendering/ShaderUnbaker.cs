using Orama.Rendering.Native;
using Orama.Rendering.Resources;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from compiled shaders to source.
/// </summary>
public static class ShaderUnbaker
{
    private static IntPtr spirVCrossContext;

    static ShaderUnbaker()
    {
        SpirVCross.InitializeImports();

        spirVCrossContext = SpirVCross.ContextCreate();
    }

    /// <summary>
    /// Converts SPIR-V vertex and fragment shaders into GLSL source.
    /// </summary>
    public static (string VertexSource, string FragmentSource) SpirVToGLSL(byte[] vertexSpirV, byte[] fragmentSpirV)
    {
        return Compile(ToUInt32Array(vertexSpirV), ToUInt32Array(fragmentSpirV), backend: 1); // 1 = GLSL
    }

    /// <summary>
    /// Converts SPIR-V vertex and fragment shaders into HLSL source.
    /// </summary>
    public static (string VertexSource, string FragmentSource) SpirVToHLSL(byte[] vertexSpirV, byte[] fragmentSpirV, uint shaderModel = 50)
    {
        return Compile(ToUInt32Array(vertexSpirV), ToUInt32Array(fragmentSpirV), backend: 2, shaderModel); // 2 = HLSL
    }

    private static uint[] ToUInt32Array(byte[] bytes)
    {
        if (bytes.Length % 4 != 0)
            throw new ArgumentException("SPIR-V byte array length must be a multiple of 4.");

        uint[] words = new uint[bytes.Length / 4];
        for (int i = 0; i < words.Length; i++)
            words[i] = BitConverter.ToUInt32(bytes, i * 4);

        return words;
    }

    private static (string VertexSource, string FragmentSource) Compile(uint[] vertexSpirV, uint[] fragmentSpirV, int backend, uint shaderModel = 0)
    {
        // Vertex shader
        IntPtr vertParsedIr = SpirVCross.ContextParseSpirv(spirVCrossContext, vertexSpirV);
        IntPtr vertCompiler = SpirVCross.ContextCreateCompiler(spirVCrossContext, vertParsedIr, backend);
        if (backend == 1 && shaderModel != 0) SpirVCross.CompilerSetHlslShaderModel(vertCompiler, shaderModel);
        string vertexSource = SpirVCross.CompilerCompile(vertCompiler);

        // Fragment shader
        IntPtr fragParsedIr = SpirVCross.ContextParseSpirv(spirVCrossContext, fragmentSpirV);
        IntPtr fragCompiler = SpirVCross.ContextCreateCompiler(spirVCrossContext, fragParsedIr, backend);
        if (backend == 1 && shaderModel != 0) SpirVCross.CompilerSetHlslShaderModel(fragCompiler, shaderModel);
        string fragmentSource = SpirVCross.CompilerCompile(fragCompiler);

        return (vertexSource, fragmentSource);
    }

    public static void Shutdown()
    {
        if (spirVCrossContext != IntPtr.Zero)
            SpirVCross.ContextDestroy(spirVCrossContext);
    }
}
