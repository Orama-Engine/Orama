// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Resources.Caches;

using Orama.Rendering.Device.Resources;

public sealed class TextureCache : ResourceCache<TextureCache, TextureKey, ITexture>
{
	/// <inheritdoc/>
	protected override ITexture Create(TextureKey key) => Renderer.Device.ResourceFactory.CreateTexture(key);
}
