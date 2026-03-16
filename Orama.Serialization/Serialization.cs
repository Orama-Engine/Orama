using Orama.Serialization.Backends;

namespace Orama.Serialization;

public static class Serialization
{
    /// <summary> The default serialization type in use when another isn't specified. </summary>
    public const SerializationType DefaultType = SerializationType.YAML;

    /// <summary> Serializes an object to a byte array. </summary>
    public static byte[] Serialize<T>(T obj, SerializationType type = DefaultType)
    {
        InstanceRepresentation rep = DataConstructor.Construct(obj);
        YAMLBackend backend = new YAMLBackend();
        return backend.Serialize(rep);
    }

    /// <summary> Deserializes a byte array to an object. </summary>
    public static T Deserialize<T>(byte[] data, SerializationType type = DefaultType) where T : new()
    {
        YAMLBackend backend = new YAMLBackend();
        return backend.Deserialize<T>(data);
    }
}
