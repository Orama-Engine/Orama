// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Xml.Linq;

using NeoVeldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceLayoutCache : ResourceCache<ResourceLayoutCache, ResourceLayoutKey, ResourceLayout>
{
	/// <inheritdoc/>
	protected override ResourceLayout Create(ResourceLayoutKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(key.Elements));

	// Very hacky way to avoid allocs
	public FrameCountedResource<ResourceLayout> GetOrCreate(ReadOnlySpan<ResourceLayoutElementDescription> span)
	{
		var hash = new HashCode();
		foreach (ref readonly var e in span)
			hash.Add(e);
		int searchHash = hash.ToHashCode();

		foreach (var kvp in Cache)
		{
			if (kvp.Key.GetHashCode() == searchHash && kvp.Key.Elements.AsSpan().SequenceEqual(span))
			{
				kvp.Value.Touch();
				return kvp.Value;
			}
		}

		var immutableElements = span.ToArray();
		var newKey = new ResourceLayoutKey(immutableElements);

		return GetOrCreate(newKey);
	}
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
