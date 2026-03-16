
namespace Orama.Serialization;

/// <summary>
/// Class responsible for converting an instance to a <see cref="InstanceRepresentation"/>.
/// </summary>
internal static class DataConstructor
{
    /// <summary> Constructs a <see cref="InstanceRepresentation"/> from an instance. </summary>
    public static InstanceRepresentation Construct(object instance)
    {
        FieldRepresentation[] fields = new FieldRepresentation[instance.GetType().GetFields().Length];
        for (int i = 0; i < fields.Length; i++)
            fields[i] = new FieldRepresentation(instance.GetType().GetFields()[i].Name, instance.GetType().GetFields()[i].GetValue(instance)!, instance.GetType().GetFields()[i].FieldType);

        return new InstanceRepresentation { Fields = fields, Type = instance.GetType() };
    }
}
