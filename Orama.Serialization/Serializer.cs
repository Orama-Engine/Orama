using Orama.Serialization.YAML;
using System.Text.Json;

namespace Orama.Serialization;

/// <summary>
/// Access to serializers.
/// </summary>
public static class Serializer
{
	public enum SerializationType
	{
		YAML,
		JSON,
		Binary
	}

	public static readonly Dictionary<SerializationType, ISerializer> Serializers = new()
	{
		{ SerializationType.YAML, new YAMLSerializer() }
	};

	/// <summary>
	/// Serialize an object.
	/// </summary>
	/// <returns>Serialized stream.</returns>
	public static Stream Serialize<T>(T value, SerializationType type = SerializationType.YAML)
	{
		return Serializers[type].Serialize(value!);
	}

	/// <summary>
	/// Deserialize an object
	/// </summary>
	/// <returns>Deserialized object</returns>
	public static T Deserialize<T>(Stream stream, SerializationType type = SerializationType.YAML)
	{
		return Serializers[type].Deserialize<T>(stream);
	}
}
