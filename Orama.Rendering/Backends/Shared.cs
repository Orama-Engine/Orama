using Orama.Rendering.Resources;
using System.Numerics;

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
        { typeof(Vector2), value =>
            {
                var v = (Vector2)value;
                byte[] bytes = new byte[8];
                Buffer.BlockCopy(BitConverter.GetBytes(v.X), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(v.Y), 0, bytes, 4, 4);
                return bytes;
            }
        },
        { typeof(Vector3), value =>
            {
                var v = (Vector3)value;
                byte[] bytes = new byte[12];
                Buffer.BlockCopy(BitConverter.GetBytes(v.X), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(v.Y), 0, bytes, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(v.Z), 0, bytes, 8, 4);
                return bytes;
            }
        },
        { typeof(Vector4), value =>
            {
                var v = (Vector4)value;
                byte[] bytes = new byte[16];
                Buffer.BlockCopy(BitConverter.GetBytes(v.X), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(v.Y), 0, bytes, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(v.Z), 0, bytes, 8, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(v.W), 0, bytes, 12, 4);
                return bytes;
            }
        },
        { typeof(GraphicsTexture), value => Array.Empty<byte>() } // Handled in the backend
    };

    /// <summary> Gets the bytes for a shader parameter. </summary>
    /// <typeparam name="T">Type of the parameter.</typeparam>
    /// <param name="param">The parameter.</param>
    /// <returns>Byte array representation of parameter.</returns>
    public static byte[] GetParameterBytes<T>(T param)
    {
        Type type = param?.GetType() ?? throw new ArgumentNullException(nameof(param));

        if (byteMappings.TryGetValue(type, out var converter))
            return converter(param!);

        throw new NotSupportedException($"Type {type} is not supported for GetParameterBytes.");
    }
}
