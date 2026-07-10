using NeoVeldrid;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceSetCache : ResourceCache<ResourceSetCache, ResourceSetKey, ResourceSet>
{
    /// <inheritdoc/>
    protected override ResourceSet Create(ResourceSetKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(key.ResourceLayout, key.BoundResources.ToArray()));
}

public readonly record struct ResourceSetKey(ResourceLayout ResourceLayout, ImmutableArray<BindableResource> BoundResources)
{
    public bool Equals(ResourceSetKey other) => BoundResources.SequenceEqual<BindableResource>(other.BoundResources);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var e in BoundResources)
            hash.Add(e);

        return hash.ToHashCode();
    }
}
