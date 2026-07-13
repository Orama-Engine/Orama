// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Collections.Immutable;

using NeoVeldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceLayoutCache : ResourceCache<ResourceLayoutCache, ResourceLayoutKey, ResourceLayout>
{
	/// <inheritdoc/>
	protected override ResourceLayout Create(ResourceLayoutKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(key.Elements.ToArray()));
}

public readonly record struct ResourceLayoutKey(ImmutableArray<ResourceLayoutElementDescription> Elements)
{
	public bool Equals(ResourceLayoutKey other) => Elements.AsSpan().SequenceEqual(other.Elements.AsSpan());

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		foreach (var e in Elements)
			hash.Add(e);

		return hash.ToHashCode();
	}
}
