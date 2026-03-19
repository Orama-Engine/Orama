
namespace Orama.Serialization.Attributes;

/// <summary>
/// Forces a property to be serialized when it typically wouldn't be.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AlwaysSerializeAttribute : Attribute { }
