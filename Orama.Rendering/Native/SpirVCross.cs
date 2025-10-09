
using System.Runtime.InteropServices;

namespace Orama.Rendering.Native;

/// <summary>
/// Native bindings for the spirv-cross shared library.
/// </summary>
internal unsafe static class SpirVCross
{
    internal static FunctionTable Imports { get; } = new(GetLibraryName());

    /// <summary> Initializes the imports for the spirv-cross shared library. </summary>
    public static void InitializeImports()
    {
        Imports.Add("spvc_context_create");
        Imports.Add("spvc_context_destroy");
        Imports.Add("spvc_context_parse_spirv");
        Imports.Add("spvc_context_create_compiler");
        Imports.Add("spvc_compiler_compile");
        Imports.Add("spvc_compiler_options_set_uint");
        Imports.Add("spvc_compiler_install_compiler_options");
        Imports.Add("spvc_compiler_create_compiler_options");
    }

    /// <summary> Returns the name of the spirv-cross shared library depending on the OS. </summary>
    private static string GetLibraryName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "spirv-cross-c-shared";

        return "libspirv-cross-c-shared";
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ContextCreateDelegate(out IntPtr context);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void ContextDestroyDelegate(IntPtr context);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ContextParseSpirvDelegate(IntPtr context, uint[] spirv, UIntPtr wordCount, out IntPtr parsedIr);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ContextCreateCompilerDelegate(IntPtr context, int backend, IntPtr parsedIr, int capturemode, out IntPtr compiler);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int CompilerCompileDelegate(IntPtr compiler, out IntPtr source);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int CompilerCreateOptionsDelegate(IntPtr compiler, out IntPtr options);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int CompilerOptionsSetUintDelegate(IntPtr options, int option, uint value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int CompilerInstallOptionsDelegate(IntPtr compiler, IntPtr options);

    public static void CompilerSetHlslShaderModel(IntPtr compiler, uint shaderModel)
    {
        var createOpts = Imports.GetFunction<CompilerCreateOptionsDelegate>("spvc_compiler_create_compiler_options");
        createOpts(compiler, out IntPtr options);

        var setUint = Imports.GetFunction<CompilerOptionsSetUintDelegate>("spvc_compiler_options_set_uint");
        setUint(options, 13, shaderModel); // 13 = SPVC_COMPILER_OPTION_HLSL_SHADER_MODEL  

        var install = Imports.GetFunction<CompilerInstallOptionsDelegate>("spvc_compiler_install_compiler_options");
        install(compiler, options);
    }

    public static IntPtr ContextCreate()
    {
        var del = Imports.GetFunction<ContextCreateDelegate>("spvc_context_create");
        del(out IntPtr ctx);
        return ctx;
    }

    public static void ContextDestroy(IntPtr context)
    {
        var del = Imports.GetFunction<ContextDestroyDelegate>("spvc_context_destroy");
        del(context);
    }

    public static IntPtr ContextParseSpirv(IntPtr context, uint[] spirv)
    {
        var del = Imports.GetFunction<ContextParseSpirvDelegate>("spvc_context_parse_spirv");
        del(context, spirv, (UIntPtr)spirv.Length, out IntPtr parsedIr);
        return parsedIr;
    }

    public static IntPtr ContextCreateCompiler(IntPtr context, IntPtr parsedIr, int backend)
    {
        var del = Imports.GetFunction<ContextCreateCompilerDelegate>("spvc_context_create_compiler");
        del(context, backend, parsedIr, 0, out IntPtr compiler);
        return compiler;
    }

    public static string CompilerCompile(IntPtr compiler)
    {
        var del = Imports.GetFunction<CompilerCompileDelegate>("spvc_compiler_compile");
        int code = del(compiler, out IntPtr ptr);
        if (code != 0)
            return code.ToString();

        return Marshal.PtrToStringAnsi(ptr) ?? "";
    }
}
