// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Runtime.CompilerServices;

namespace Orama.Rendering.Resources.Caches;

public sealed class TextureViewCache : ResourceCache<TextureViewCache, TextureViewKey, NeoVeldrid.TextureView>
{
	/// <inheritdoc/>
	protected override NeoVeldrid.TextureView Create(TextureViewKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateTextureView(key.Texture);
}


public readonly record struct TextureViewKey(NeoVeldrid.Texture Texture)
{
	public bool Equals(TextureViewKey other) => ReferenceEquals(Texture, other.Texture);

	public override int GetHashCode() => RuntimeHelpers.GetHashCode(Texture);
}
