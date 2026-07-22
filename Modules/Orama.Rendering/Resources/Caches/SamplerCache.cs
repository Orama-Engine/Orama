// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device.Resources;


namespace Orama.Rendering.Resources.Caches;

public sealed class SamplerCache : ResourceCache<SamplerCache, SamplerKey, ISampler>
{
	/// <inheritdoc/>
	protected override ISampler Create(SamplerKey key) => Renderer.Device.ResourceFactory.CreateSampler(key);
}
