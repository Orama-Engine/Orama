using Orama.Components;

namespace Orama.Entities;

// TODO: Maybe move this to component attributes?

/// <summary>
/// Marks a <see cref="Component"/> variable within an <see cref="Entity"/> as implicit.
/// </summary>
/// <remarks>
/// Implicit Components will be added to the Entity automatically when the Entity is created
/// and are hidden in the inspector.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ImplicitComponentAttribute : Attribute { }
