using Orama.Rendering.Resources;
using Vortice.ShaderCompiler;
using Vortice.SpirvCross;
using static Vortice.SpirvCross.SpirvCrossApi;

namespace Orama.Rendering;

/// <summary>
/// Responsible for converting shaders from compiled shaders to source.
/// </summary>
public static class ShaderUnbaker
{
    /// <summary> Converts SPIR-V vertex and fragment shaders into GLSL source. </summary>
    public static (string VertexSource, string FragmentSource) SpirVToGLSL(byte[] vertexSpirV, byte[] fragmentSpirV)
    {
        string vertexSource = Compile(vertexSpirV, Backend.GLSL);
        string fragmentSource = Compile(fragmentSpirV, Backend.GLSL);
        return (vertexSource, fragmentSource);
    }

    /// <summary> Converts SPIR-V vertex and fragment shaders into HLSL source. </summary>
    public static (string VertexSource, string FragmentSource) SpirVToHLSL(byte[] vertexSpirV, byte[] fragmentSpirV, uint shaderModel = 50)
    {
        string vertexSource = Compile(vertexSpirV, Backend.HLSL, shaderModel);
        string fragmentSource = Compile(fragmentSpirV, Backend.HLSL, shaderModel);
        return (vertexSource, fragmentSource);
    }

    private static string Compile(byte[] spirvBytes, Backend backend, uint shaderModel = 0)
    {
        // Create SPIRV-Cross context
        spvc_context_create(out spvc_context context).CheckResult();
        try
        {
            // Parse SPIR-V
            spvc_context_parse_spirv(context, spirvBytes, out spvc_parsed_ir parsedIr).CheckResult();

            // Create compiler
            spvc_context_create_compiler(context, backend, parsedIr, CaptureMode.TakeOwnership, out spvc_compiler compiler).CheckResult();

            // Configure options
            spvc_compiler_create_compiler_options(compiler, out spvc_compiler_options options).CheckResult();
            if (backend == Backend.GLSL)
            {
                spvc_compiler_options_set_uint(options, CompilerOption.GLSLVersion, 450);
            }
            else if (backend == Backend.HLSL && shaderModel != 0)
            {
                spvc_compiler_options_set_uint(options, CompilerOption.HLSLShaderModel, shaderModel);
            }
            spvc_compiler_install_compiler_options(compiler, options);

            // Compile to source
            spvc_compiler_compile(compiler, out string? source);
            return source ?? throw new Exception("Failed to compile SPIR-V to source.");
        }
        finally
        {
            spvc_context_release_allocations(context);
            spvc_context_destroy(context);
        }
    }
}
