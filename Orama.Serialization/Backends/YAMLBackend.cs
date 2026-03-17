using System.Text;

namespace Orama.Serialization.Backends;

[SerializerBackend(SerializationType.YAML)]
internal class YAMLBackend : SerializerBackend
{
    /// <inheritdoc/>
    public override InstanceRepresentation Deserialize(byte[] data)
    {
        List<FieldRepresentation> fields = new List<FieldRepresentation>();

        StringReader reader = new StringReader(Encoding.UTF8.GetString(data));

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine()!;

            if (line.StartsWith("#"))
                continue;

            string[] parts = line.Split(':', 2);
            if (parts.Length < 2) continue;

            string name = parts[0].Trim();
            string value = parts[1].Trim();

            fields.Add(new FieldRepresentation() { Name = name, Value = value });
        }

        return new InstanceRepresentation() { Fields = fields.ToArray() };
    }


    /// <inheritdoc/>
    public override byte[] Serialize(InstanceRepresentation obj)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var field in obj.Fields)
            sb.AppendLine($"{field.Name}: {field.Value}");

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
