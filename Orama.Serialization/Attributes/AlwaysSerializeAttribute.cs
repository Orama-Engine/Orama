
namespace Orama.Serialization.Attributes;

/// <summary>
/// Forces a property or field to be serialized when it typically wouldn't be.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class AlwaysSerializeAttribute : Attribute { }
