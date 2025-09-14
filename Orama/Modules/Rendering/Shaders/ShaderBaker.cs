using Veldrid;
using Veldrid.SPIRV;

namespace Orama.Rendering.Shaders;

/// <summary>
/// Provides methods for compiling GLSL shader source code into SPIR-V bytecode.
/// </summary>
internal static class ShaderBaker
{
	/// <summary>
	/// Compiles GLSL vertex and fragment shader sources to SPIR-V bytecode.
	/// </summary>
	/// <param name="vertexSource">GLSL vertex shader source code.</param>
	/// <param name="fragmentSource">GLSL fragment shader source code.</param>
	/// <returns>A tuple containing the compiled SPIR-V bytecode for vertex and fragment shaders.</returns>
	public static (byte[] VertexSpirv, byte[] FragmentSpirv) Bake(string vertexSource, string fragmentSource)
	{
		// Compile both using Veldrid.SPIRV
		var vertexResult = SpirvCompilation.CompileGlslToSpirv(
			vertexSource, "vertex.glsl", ShaderStages.Vertex, new GlslCompileOptions());

		var fragmentResult = SpirvCompilation.CompileGlslToSpirv(
			fragmentSource, "fragment.glsl", ShaderStages.Fragment, new GlslCompileOptions());

		if (vertexResult == null || fragmentResult == null)
			throw new Exception($"Shader compilation failed");

		return (vertexResult.SpirvBytes, fragmentResult.SpirvBytes);
	}

	/// <summary>
	/// Compiles a single GLSL shader source to SPIR-V bytecode.
	/// </summary>
	/// <param name="source">GLSL shader source code.</param>
	/// <param name="stage">Shader stage (Vertex, Fragment, etc.).</param>
	/// <returns>SPIR-V bytecode.</returns>
	public static byte[] BakeSingle(string source, ShaderStages stage)
	{
		var result = SpirvCompilation.CompileGlslToSpirv(
			source, $"{stage.ToString().ToLower()}.glsl", stage, new GlslCompileOptions());

		if (result == null)
			throw new Exception($"Shader compilation failed");

		return result.SpirvBytes;
	}
}