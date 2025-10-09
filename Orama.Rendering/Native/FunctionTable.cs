using System.Runtime.InteropServices;

namespace Orama.Rendering.Native;

/// <summary>
/// A Table of function names to native pointers.
/// </summary>
internal class FunctionTable
{
    private Dictionary<string, IntPtr> table = new();
    private IntPtr library;

    public FunctionTable(string libraryName) => library = NativeLibrary.Load(Path.Combine(GetRuntimeFolder(), "native", libraryName));

    /// <summary> Adds a function to the table. </summary>
    public void Add(string name)
    {
        IntPtr ptr = NativeLibrary.GetExport(library, name);
        table.Add(name, ptr);
    }

    /// <summary> Removes a function from the table. </summary>
    public void Remove(string name) => table.Remove(name);

    /// <summary>
    /// Gets the native pointer for a function by name.
    /// </summary>
    public IntPtr GetFunction(string name)
    {
        if (!table.TryGetValue(name, out IntPtr ptr))
            throw new KeyNotFoundException($"Function '{name}' is not registered in the function table.");

        return ptr;
    }

    /// <summary>
    /// Gets a typed delegate for a function.
    /// </summary>
    public TDelegate GetFunction<TDelegate>(string name) where TDelegate : Delegate
    {
        IntPtr ptr = GetFunction(name);
        return Marshal.GetDelegateForFunctionPointer<TDelegate>(ptr);
    }

    /// <summary> Returns the runtime folder. </summary>
    private static string GetRuntimeFolder()
    {
        string runtimes = Path.Combine(AppContext.BaseDirectory, "runtimes");
        string osFolder = Path.Combine(runtimes, RuntimeInformation.RuntimeIdentifier);

        return osFolder;
    }
}
