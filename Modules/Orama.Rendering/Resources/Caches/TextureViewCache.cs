// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Runtime.CompilerServices;

namespace Orama.Rendering.Resources.Caches;

using Orama.Rendering.Device.Resources;

public sealed class TextureViewCache : ResourceCache<TextureViewCache, TextureViewKey, ITextureView>
{
	/// <inheritdoc/>
	protected override ITextureView Create(TextureViewKey key) => Renderer.Device.ResourceFactory.CreateTextureView(key);
}

public readonly ref struct TextureViewKey(ITexture texture) : IResourceKey
{
	public readonly ITexture Texture = texture;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(TextureViewKey other) => ReferenceEquals(Texture, other.Texture);

	/// <inheritdoc/>
	public override int GetHashCode() => RuntimeHelpers.GetHashCode(Texture);
}
