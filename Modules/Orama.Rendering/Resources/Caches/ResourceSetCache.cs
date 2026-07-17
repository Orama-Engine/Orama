// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Veldrith;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceSetCache : ResourceCache<ResourceSetCache, ResourceSetKey, ResourceSet>
{
	/// <inheritdoc/>
	protected override ResourceSet Create(ResourceSetKey key) => Renderer.Veldrith.GraphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(key.ResourceLayout, key.BoundResources.ToArray()));
}

public readonly ref struct ResourceSetKey(ResourceLayout resourceLayout, ReadOnlySpan<IBindableResource> boundResources) : IResourceKey
{
	public readonly ResourceLayout ResourceLayout = resourceLayout;
	public readonly ReadOnlySpan<IBindableResource> BoundResources = boundResources;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(ResourceSetKey other)
	{
		if (ResourceLayout != other.ResourceLayout)
			return false;
		if (BoundResources.Length != other.BoundResources.Length)
			return false;
		for (int i = 0; i < BoundResources.Length; i++)
		{
			if (!BoundResources[i].Equals(other.BoundResources[i]))
				return false;
		}

		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + (ResourceLayout?.GetHashCode() ?? 0);
			foreach (var e in BoundResources)
				hash = hash * 31 + (e?.GetHashCode() ?? 0);
			return hash;
		}
	}
}
