
namespace Orama.Core.Common.Utility;

/// <summary>
/// Marks a static method to be ran automatically when the assembly is loaded.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OnAssemblyLoadAttribute : Attribute { }
