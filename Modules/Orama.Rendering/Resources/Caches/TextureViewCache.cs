// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System;
using System.Runtime.CompilerServices;

namespace Orama.Rendering.Resources.Caches;

public sealed class TextureViewCache : ResourceCache<TextureViewCache, TextureViewKey, Veldrith.TextureView>
{
	/// <inheritdoc/>
	protected override Veldrith.TextureView Create(TextureViewKey key) => Renderer.Veldrith.GraphicsDevice.ResourceFactory.CreateTextureView(key.Texture);
}

public readonly ref struct TextureViewKey(Veldrith.Texture texture) : IResourceKey
{
	public readonly Veldrith.Texture Texture = texture;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(TextureViewKey other) => ReferenceEquals(Texture, other.Texture);

	/// <inheritdoc/>
	public override int GetHashCode() => RuntimeHelpers.GetHashCode(Texture);
}
