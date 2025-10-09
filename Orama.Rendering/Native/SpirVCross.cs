
using System.Runtime.InteropServices;

namespace Orama.Rendering.Native;

/// <summary>
/// Native bindings for the spirv-cross shared library.
/// </summary>
internal static class SpirVCross
{
    internal static FunctionTable Imports { get; } = new(GetLibraryName());

    /// <summary> Initializes the imports for the spirv-cross shared library. </summary>
    public static void InitializeImports()
    {

    }

    /// <summary> Returns the name of the spirv-cross shared library depending on the OS. </summary>
    private static string GetLibraryName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "spirv-cross-c-shared";

        return "libspirv-cross-c-shared";
    }
}
