using Orama.Rendering.Resources;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Orama.Rendering.Backends;

/// <summary>
/// Shared utilities between all backends.
/// </summary>
public static class Shared
{
    private readonly static Dictionary<Type, Func<object, byte[]>> byteMappings = new Dictionary<Type, Func<object, byte[]>>()
    {
        { typeof(float), value => BitConverter.GetBytes((float)value) },
        { typeof(int), value => BitConverter.GetBytes((int)value) },
        { typeof(GraphicsTexture), value => Array.Empty<byte>() } // Handled in the backend
    };

    /// <summary> Gets the bytes for a shader parameter. </summary>
    /// <typeparam name="T">Type of the parameter.</typeparam>
    /// <param name="param">The parameter.</param>
    /// <returns>Byte array representation of parameter.</returns>
    public static byte[] GetParameterBytes<T>(T param)
    {
        if (param == null) throw new ArgumentNullException(nameof(param));
        Type type = param.GetType();

        if (byteMappings.TryGetValue(type, out var converter))
            return converter(param!);

        // If it's a struct, marshal it
        if (type.IsValueType && !type.IsPrimitive)
            return MarshalStruct((dynamic)param);

        throw new NotSupportedException($"Type {type} is not supported for GetParameterBytes.");
    }

    private static byte[] MarshalStruct<T>(T str) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(str, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            return arr;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
