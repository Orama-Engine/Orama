// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Collections.Immutable;
using System.Xml.Linq;

using NeoVeldrid;

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
