
namespace Orama.Core.Common.Resources;

/// <summary>
/// A Provider for game resources/assets.
/// </summary>
public interface IResourceProvider
{
    /// <summary> Gets a resource from its path. </summary>
    /// <returns> The resource or null if invalid. </returns>
    T? GetResource<T>(string path) where T : class;
}
