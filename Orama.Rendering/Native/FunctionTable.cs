
using System.Reflection;
using System.Runtime.InteropServices;

namespace Orama.Rendering.Native;

/// <summary>
/// A Table of function ids to native pointers.
/// </summary>
internal class FunctionTable
{
    private Dictionary<int, IntPtr> table = new();
    private IntPtr library;

    public FunctionTable(string libraryName) => library = NativeLibrary.Load(Path.Combine(GetRuntimeFolder(), "native", libraryName));

    /// <summary> Adds a function key and function to the table. </summary>
    public void Add(int key, string name)
    {
        IntPtr ptr = NativeLibrary.GetExport(library, name);
        table.Add(key, ptr);
    }

    /// <summary> Removes a function name and pointer from the table. </summary>
    public void Remove(int key) => table.Remove(key);

    /// <summary> Returns the runtime folder. </summary>
    private static string GetRuntimeFolder()
    {
        string runtimes = Path.Combine(AppContext.BaseDirectory, "runtimes");
        string osFolder = Path.Combine(runtimes, RuntimeInformation.RuntimeIdentifier);

        return osFolder;
    }

    public IntPtr this[int key] => table[key];
}
