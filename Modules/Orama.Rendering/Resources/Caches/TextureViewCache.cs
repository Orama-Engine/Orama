// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Resources.Caches;

using Orama.RHI.Resources;

public sealed class TextureViewCache : ResourceCache<TextureViewCache, TextureViewDescriptor, ITextureView>
{
	/// <inheritdoc/>
	protected override ITextureView Create(TextureViewDescriptor key) => Renderer.Device.ResourceFactory.CreateTextureView(key);
}
