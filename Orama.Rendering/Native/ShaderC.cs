
using System.Runtime.InteropServices;

namespace Orama.Rendering.Native;

internal enum ShaderKind
{
    VertexShader = 0,
    FragmentShader = 1,
    ComputeShader = 2,
    GeometryShader = 3,
    TessControlShader = 4,
    TessEvaluationShader = 5
}

/// <summary>
/// Native bindings for the shaderc shared library.
/// </summary>
internal static class ShaderC
{
    internal static FunctionTable Imports { get; } = new(GetLibraryName());

    /// <summary> Initializes the imports for the shaderc shared library. </summary>
    public static void InitializeImports()
    {
        Imports.Add("shaderc_compile_into_spv");
    }

    /// <summary> Returns the name of the shaderc shared library depending on the OS. </summary>
    public static string GetLibraryName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "shaderc_shared";

        return "libshaderc_shared";
    }

}
