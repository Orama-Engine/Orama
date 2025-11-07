using Vortice.ShaderCompiler;
using Orama.Rendering.Resources;
using System.Collections.Concurrent;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from source to compiled shaders.
/// </summary>
public static class ShaderBaker
{
    private static readonly Compiler compiler = new();
    private static readonly ConcurrentDictionary<ulong, GraphicsShader> cache = new();

    static ShaderBaker()
    {
        compiler = new();
    }

    /// <summary> Compiles GLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader GLSLToShader(string vertex, string fragment) => CompileCached(vertex, fragment, SourceLanguage.GLSL);

    /// <summary> Compiles HLSL source to a <see cref="GraphicsShader"/>. </summary>
    public static GraphicsShader HLSLToShader(string vertex, string fragment) => CompileCached(vertex, fragment, SourceLanguage.HLSL);

    private static GraphicsShader CompileCached(string vert, string frag, SourceLanguage lang)
    {
        ulong key = ComputeKey(vert, frag, lang);

        if (cache.TryGetValue(key, out var existing))
            return existing;

        var shader = CompileFresh(vert, frag, lang);
        cache[key] = shader;
        return shader;
    }

    private static GraphicsShader CompileFresh(string vertexSource, string fragmentSource, SourceLanguage sourceLang)
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

        return new GraphicsShader
        {
            VertexBytes = vert.Bytecode,
            FragmentBytes = frag.Bytecode
        };
    }

    private static ulong ComputeKey(string vert, string frag, SourceLanguage lang)
    {
        ulong hash = 1469598103934665603ul;

        void HashString(string s)
        {
            foreach (var c in s)
                hash = (hash ^ (byte)c) * 1099511628211ul;
        }

        HashString(vert);
        HashString(frag);

        unchecked
        {
            hash ^= (ulong)lang << 32;
        }

        return hash;
    }
}
