using Veldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceSetCache : ResourceCache<ResourceSetCache, ResourceSetKey, ResourceSet>
{
    /// <inheritdoc/>
    protected override ResourceSet Create(ResourceSetKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(key.ResourceLayout, [key.BoundResource0]));
}

// TODO: Immutable array comparison
public readonly record struct ResourceSetKey(ResourceLayout ResourceLayout, BindableResource BoundResource0);
