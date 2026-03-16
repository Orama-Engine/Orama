
namespace Orama.Serialization.Backends;

internal abstract class SerializerBackend
{
    /// <summary> Serializes the specified object. </summary>
    public abstract byte[] Serialize(InstanceRepresentation obj);

    /// <summary> Deserializes the specified data. </summary>
    public abstract T Deserialize<T>(byte[] data) where T : new();
}
