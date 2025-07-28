using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Orama.Serialization.JSON;

public class JSONSerializer : ISerializer
{
	public T Deserialize<T>(Stream stream)
	{
		throw new NotImplementedException();
	}

	public Stream Serialize<T>(T value)
	{
		if (value == null)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes("null"));
		}

		var options = new JsonSerializerOptions
		{
			WriteIndented = true,
			IgnoreReadOnlyProperties = true
		};

		var propertiesToSerialize = GetPropertiesToSerialize(typeof(T));
		var fieldsToSerialize = GetFieldsToSerialize(typeof(T));

		var memoryStream = new MemoryStream();
		var writer = new Utf8JsonWriter(memoryStream, new JsonWriterOptions { Indented = options.WriteIndented });

		writer.WriteStartObject();

		foreach (var property in propertiesToSerialize)
		{
			var propertyValue = property.GetValue(value);
			writer.WritePropertyName(property.Name);
			JsonSerializer.Serialize(writer, propertyValue, options);
		}

		foreach (var field in fieldsToSerialize)
		{
			var fieldValue = field.GetValue(value);
			writer.WritePropertyName(field.Name);
			JsonSerializer.Serialize(writer, fieldValue, options);
		}

		writer.WriteEndObject();
		writer.Flush();

		memoryStream.Position = 0;
		return memoryStream;
	}

	private IEnumerable<PropertyInfo> GetPropertiesToSerialize(Type type)
	{
		return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.CanRead &&
					   !Attribute.IsDefined(p, typeof(DontSerializeAttribute)) &&
					   (p.GetGetMethod()?.IsPublic == true ||
						Attribute.IsDefined(p, typeof(SerializeAttribute))));
	}

	private IEnumerable<FieldInfo> GetFieldsToSerialize(Type type)
	{
		return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(f => !Attribute.IsDefined(f, typeof(DontSerializeAttribute)) &&
					   (f.IsPublic || Attribute.IsDefined(f, typeof(SerializeAttribute))));
	}
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class SerializeAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class DontSerializeAttribute : Attribute { }