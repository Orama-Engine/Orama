// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using NeoVeldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceSetCache : ResourceCache<ResourceSetCache, ResourceSetKey, ResourceSet>
{
	/// <inheritdoc/>
	protected override ResourceSet Create(ResourceSetKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(key.ResourceLayout, key.BoundResources.ToArray()));

	// Very hacky way to avoid allocs
	public FrameCountedResource<ResourceSet> GetOrCreate(ResourceLayout layout, ReadOnlySpan<BindableResource> span)
	{
		var hash = new HashCode();
		foreach (ref readonly var e in span)
			hash.Add(e);
		int searchHash = hash.ToHashCode();

		foreach (var kvp in Cache)
		{
			if (kvp.Key.GetHashCode() == searchHash && kvp.Key.BoundResources.SequenceEqual(span))
			{
				kvp.Value.Touch();
				return kvp.Value;
			}
		}

		var immutableElements = span.ToArray();
		var newKey = new ResourceSetKey(layout, immutableElements);

		return GetOrCreate(newKey);
	}
}

public readonly record struct ResourceSetKey(ResourceLayout ResourceLayout, BindableResource[] BoundResources)
{
	public bool Equals(ResourceSetKey other) => BoundResources.SequenceEqual(other.BoundResources);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		foreach (var e in BoundResources)
			hash.Add(e);

		return hash.ToHashCode();
	}
}
