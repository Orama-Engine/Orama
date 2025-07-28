using Orama.Serialization.Attributes;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Orama.Serialization.YAML
{
	internal class YAMLSerializer : ISerializer
	{
		private readonly IDeserializer _deserializer;
		private readonly YamlDotNet.Serialization.ISerializer _serializer;

		public YAMLSerializer()
		{
			_deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();

			_serializer = new SerializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
				.Build();
		}

		public T Deserialize<T>(Stream stream)
		{
			using var reader = new StreamReader(stream);
			var yaml = reader.ReadToEnd();
			return _deserializer.Deserialize<T>(yaml);
		}

		public Stream Serialize<T>(T value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			// Filter properties and fields according to rules
			var propertiesToSerialize = typeof(T)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.CanRead &&
						   p.GetCustomAttribute<DontSerializeAttribute>() == null)
				.ToList();

			var fieldsToSerialize = typeof(T)
				.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(f => f.GetCustomAttribute<SerializeAttribute>() != null &&
						  f.GetCustomAttribute<DontSerializeAttribute>() == null)
				.ToList();

			var privatePropertiesToSerialize = typeof(T)
				.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(p => p.CanRead &&
						  p.GetCustomAttribute<SerializeAttribute>() != null &&
						  p.GetCustomAttribute<DontSerializeAttribute>() == null)
				.ToList();

			// Create a dynamic object with only the members we want to serialize
			var dynamicObject = new Dictionary<string, object>();

			foreach (var prop in propertiesToSerialize)
			{
				dynamicObject[prop.Name] = prop.GetValue(value);
			}

			foreach (var field in fieldsToSerialize)
			{
				dynamicObject[field.Name] = field.GetValue(value);
			}

			foreach (var prop in privatePropertiesToSerialize)
			{
				dynamicObject[prop.Name] = prop.GetValue(value);
			}

			var yaml = _serializer.Serialize(dynamicObject);
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(yaml);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}