// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Collections.Immutable;

using NeoVeldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceLayoutCache : ResourceCache<ResourceLayoutCache, ResourceLayoutKey, ResourceLayout>
{
	/// <inheritdoc/>
	protected override ResourceLayout Create(ResourceLayoutKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(key.Elements));
}

public readonly record struct ResourceLayoutKey(ResourceLayoutElementDescription[] Elements)
{
	public bool Equals(ResourceLayoutKey other) => Elements.SequenceEqual(other.Elements);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		foreach (var e in Elements)
			hash.Add(e);

		return hash.ToHashCode();
	}
}
