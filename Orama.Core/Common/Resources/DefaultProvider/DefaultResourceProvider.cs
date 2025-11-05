using Orama.Core.Common.Resources.DefaultProvider;

namespace Orama.Core.Common.Resources.Default;

internal class DefaultResourceProvider : IResourceProvider
{
    /// <inheritdoc/>
    public T? GetResource<T>(string path) where T : class
    {
        if (!File.Exists(path))
            return null;

        byte[] data = File.ReadAllBytes(path);
        return ResourceLoader<T>.GetResourceLoader()?.LoadResource(data);
    }
}
