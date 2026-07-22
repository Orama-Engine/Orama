// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Resources.Caches;

using Orama.Rendering.Device.Resources;

public sealed class TextureCache : ResourceCache<TextureCache, TextureDescriptor, ITexture>
{
	/// <inheritdoc/>
	protected override ITexture Create(TextureDescriptor key) => Renderer.Device.ResourceFactory.CreateTexture(key);
}
