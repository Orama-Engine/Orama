
namespace Orama.Serialization.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SerializeAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DontSerializeAttribute : Attribute
{
}
