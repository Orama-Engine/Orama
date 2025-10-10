#if DEBUG
namespace Orama.Core.Common.Entities;

/// <summary>
/// Internal entity useful for debugging systems.
/// </summary>
internal class DebugEntity : BaseEntity
{
    public override void Start()
    {
        Console.WriteLine("Debug entity started.");
    }
}

#endif