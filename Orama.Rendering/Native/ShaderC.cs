
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

internal enum SourceLanguage
{
    GLSL = 0,
    HLSL = 1
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
        Imports.Add("shaderc_compiler_initialize");
        Imports.Add("shaderc_compile_options_initialize");
        Imports.Add("shaderc_compile_into_spv");
        Imports.Add("shaderc_result_get_compilation_status");
        Imports.Add("shaderc_result_get_error_message");
        Imports.Add("shaderc_result_get_bytes");
        Imports.Add("shaderc_result_get_length");
        Imports.Add("shaderc_result_release");
        Imports.Add("shaderc_compile_options_set_source_language");
    }

    /// <summary> Returns the name of the shaderc shared library depending on the OS. </summary>
    private static string GetLibraryName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "shaderc_shared";

        return "libshaderc_shared";
    }

    private delegate IntPtr CompilerInitializeDelegate();
    private delegate IntPtr CompileOptionsInitializeDelegate();
    private delegate IntPtr CompileIntoSpvDelegate(
        IntPtr compiler,
        string sourceText,
        UIntPtr sourceLength,
        ShaderKind kind,
        string inputFileName,
        string entryPointName,
        IntPtr options
    );
    private delegate int ResultGetCompilationStatusDelegate(IntPtr result);
    private delegate IntPtr ResultGetErrorMessageDelegate(IntPtr result);
    private delegate IntPtr ResultGetBytesDelegate(IntPtr result);
    private delegate UIntPtr ResultGetLengthDelegate(IntPtr result);
    private delegate void ResultReleaseDelegate(IntPtr result);
    private delegate void CompileOptionsSetSourceLanguageDelegate(IntPtr options, SourceLanguage language);

    public static IntPtr CompilerInitialize()
    {
        var del = Imports.GetFunction<CompilerInitializeDelegate>("shaderc_compiler_initialize");
        return del();
    }

    public static IntPtr CompileOptionsInitialize()
    {
        var del = Imports.GetFunction<CompileOptionsInitializeDelegate>("shaderc_compile_options_initialize");
        return del();
    }

    public static IntPtr CompileIntoSpv(IntPtr compiler, string sourceText, ShaderKind kind, string inputFileName, string entryPointName, IntPtr options)
    {
        var del = Imports.GetFunction<CompileIntoSpvDelegate>("shaderc_compile_into_spv");
        return del(compiler, sourceText, (UIntPtr)sourceText.Length, kind, inputFileName, entryPointName, options);
    }

    public static int ResultGetCompilationStatus(IntPtr result)
    {
        var del = Imports.GetFunction<ResultGetCompilationStatusDelegate>("shaderc_result_get_compilation_status");
        return del(result);
    }

    public static string ResultGetErrorMessage(IntPtr result)
    {
        var del = Imports.GetFunction<ResultGetErrorMessageDelegate>("shaderc_result_get_error_message");
        IntPtr ptr = del(result);
        return Marshal.PtrToStringAnsi(ptr) ?? "";
    }

    public static byte[] ResultGetBytes(IntPtr result)
    {
        var delBytes = Imports.GetFunction<ResultGetBytesDelegate>("shaderc_result_get_bytes");
        var delLength = Imports.GetFunction<ResultGetLengthDelegate>("shaderc_result_get_length");

        UIntPtr length = delLength(result);
        byte[] spirv = new byte[(int)length];
        IntPtr ptr = delBytes(result);
        Marshal.Copy(ptr, spirv, 0, (int)length);
        return spirv;
    }

    public static void ResultRelease(IntPtr result)
    {
        var del = Imports.GetFunction<ResultReleaseDelegate>("shaderc_result_release");
        del(result);
    }

    public static void CompileOptionsSetSourceLanguage(IntPtr options, SourceLanguage language)
    {
        var del = Imports.GetFunction<CompileOptionsSetSourceLanguageDelegate>("shaderc_compile_options_set_source_language");
        del(options, language);
    }
}
