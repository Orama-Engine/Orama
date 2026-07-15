// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System;
using System.Xml.Linq;

using NeoVeldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceLayoutCache : ResourceCache<ResourceLayoutCache, ResourceLayoutKey, ResourceLayout>
{
	/// <inheritdoc/>
	protected override ResourceLayout Create(ResourceLayoutKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(key.Elements.ToArray()));
}

public readonly ref struct ResourceLayoutKey(ReadOnlySpan<ResourceLayoutElementDescription> elements) : IResourceKey
{
	public readonly ReadOnlySpan<ResourceLayoutElementDescription> Elements = elements;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(ResourceLayoutKey other)
	{
		if (Elements.Length != other.Elements.Length) return false;
		for (int i = 0; i < Elements.Length; i++) if (!Elements[i].Equals(other.Elements[i])) return false;
		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			foreach (var e in Elements)
			{
				hash = hash * 31 + (e.Name?.GetHashCode() ?? 0);
				hash = hash * 31 + (int)e.Kind;
				hash = hash * 31 + (int)e.Stages;
				hash = hash * 31 + (int)e.Options;
			}
			return hash;
		}
	}
}
