
namespace Orama.Serialization.Attributes;

/// <summary>
/// Forces a property to never serialize even if it would otherwise be serialized.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DontSerializeAttribute : Attribute { }
