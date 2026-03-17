
namespace Orama.Serialization.Backends;

/// <summary>
/// Marks a class as a serializer backend.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class SerializerBackendAttribute : Attribute
{
    /// <summary> The serialization type this backend is for. </summary>
    public SerializationType SerializationType { get; set; }

    /// <summary> Initializes a new instance of the <see cref="SerializerBackendAttribute"/> class. </summary>
    public SerializerBackendAttribute(SerializationType serializationType) => SerializationType = serializationType;
}

internal abstract class SerializerBackend
{
    /// <summary> Serializes the specified object. </summary>
    public abstract byte[] Serialize(InstanceRepresentation obj);

    /// <summary> Deserializes the specified data. </summary>
    public abstract T Deserialize<T>(byte[] data) where T : new();
}
