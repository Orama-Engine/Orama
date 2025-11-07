using System.Collections.Concurrent;
using Vortice.SpirvCross;
using static Vortice.SpirvCross.SpirvCrossApi;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from compiled shaders to source.
/// </summary>
public static class ShaderUnbaker
{
    private static readonly ConcurrentDictionary<ulong, string> cache = new();

    /// <summary> Converts SPIR-V vertex and fragment shaders into GLSL source. </summary>
    public static (string VertexSource, string FragmentSource) SpirVToGLSL(byte[] vertexSpirV, byte[] fragmentSpirV)
    {
        return (CompileCached(vertexSpirV, Backend.GLSL, 0), CompileCached(fragmentSpirV, Backend.GLSL, 0));
    }

    /// <summary> Converts SPIR-V vertex and fragment shaders into HLSL source. </summary>
    public static (string VertexSource, string FragmentSource) SpirVToHLSL(byte[] vertexSpirV, byte[] fragmentSpirV, uint shaderModel = 50)
    {
        return (CompileCached(vertexSpirV, Backend.HLSL, shaderModel), CompileCached(fragmentSpirV, Backend.HLSL, shaderModel));
    }

    private static string CompileCached(ReadOnlySpan<byte> spirv, Backend backend, uint shaderModel)
    {
        ulong key = ComputeKey(spirv, backend, shaderModel);

        if (cache.TryGetValue(key, out var existing))
            return existing;

        var source = CompileFresh(spirv, backend, shaderModel);
        cache[key] = source;
        return source;
    }

    private static string CompileFresh(ReadOnlySpan<byte> spirvBytes, Backend backend, uint shaderModel)
    {
        spvc_context_create(out spvc_context context).CheckResult();

        try
        {
            spvc_context_parse_spirv(context, spirvBytes, out spvc_parsed_ir ir).CheckResult();
            spvc_context_create_compiler(context, backend, ir, CaptureMode.TakeOwnership, out spvc_compiler comp).CheckResult();

            spvc_compiler_create_compiler_options(comp, out spvc_compiler_options opts).CheckResult();

            if (backend == Backend.GLSL)
                spvc_compiler_options_set_uint(opts, CompilerOption.GLSLVersion, 450);
            else if (backend == Backend.HLSL)
                spvc_compiler_options_set_uint(opts, CompilerOption.HLSLShaderModel, shaderModel);

            spvc_compiler_install_compiler_options(comp, opts);
            spvc_compiler_compile(comp, out string? src);

            return src ?? throw new Exception("SPIR-V conversion failed.");
        }
        finally
        {
            spvc_context_release_allocations(context);
            spvc_context_destroy(context);
        }
    }

    private static ulong ComputeKey(ReadOnlySpan<byte> spirv, Backend backend, uint shaderModel)
    {
        ulong hash = 1469598103934665603ul;
        foreach (byte b in spirv)
            hash = (hash ^ b) * 1099511628211ul;

        unchecked
        {
            hash ^= (ulong)backend;
            hash ^= shaderModel << 32;
        }

        return hash;
    }
}
