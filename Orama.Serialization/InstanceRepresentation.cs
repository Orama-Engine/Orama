
namespace Orama.Serialization;

/// <summary>
/// Data representation of a field.
/// </summary>
internal record struct FieldRepresentation(string Name, object Value, Type Type);

/// <summary>
/// Data representation of an instance.
/// </summary>
/// <remarks>
/// We use this instead of always reflecting for caching + other reasons.
/// </remarks>
internal struct InstanceRepresentation
{
    /// <summary> The type of the instance. </summary>
    public Type Type { get; set; } 

    /// <summary> All serialized fields of the instance. </summary>
    public FieldRepresentation[] Fields { get; set; }
}
