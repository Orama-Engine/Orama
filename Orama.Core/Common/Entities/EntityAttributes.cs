
namespace Orama.Core.Common.Entities;

/// <summary>
/// Indicates that the property or field of the entity is an implicit component.
/// </summary>
/// <remarks> An implicit component is a component that is automatically added to the <see cref="Entity"/> upon creation. </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ImplicitComponent : Attribute { }
