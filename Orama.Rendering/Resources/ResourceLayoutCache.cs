using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;

namespace Orama.Rendering.Resources;

public readonly record struct ResourceLayoutDescriptor(ResourceLayoutElementDescription[] Elements)
{
    public bool Equals(ResourceLayoutDescriptor other) => Elements.AsSpan().SequenceEqual(other.Elements);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var e in Elements)
            hash.Add(e);

        return hash.ToHashCode();
    }
}

public sealed class ResourceLayoutCache : ResourceCache<ResourceLayoutCache, ResourceLayoutDescriptor, ResourceLayout>
{
    /// <inheritdoc/>
    protected override ResourceLayout Create(ResourceLayoutDescriptor key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(key.Elements));
}
