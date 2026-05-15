using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceLayoutCache : ResourceCache<ResourceLayoutCache, ResourceLayoutKey, ResourceLayout>
{
    /// <inheritdoc/>
    protected override ResourceLayout Create(ResourceLayoutKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(key.Elements));
}

public readonly record struct ResourceLayoutKey(ResourceLayoutElementDescription[] Elements)
{
    public bool Equals(ResourceLayoutKey other) => Elements.AsSpan().SequenceEqual(other.Elements);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var e in Elements)
            hash.Add(e);

        return hash.ToHashCode();
    }
}
