// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.RHI.Resources;


namespace Orama.Rendering.Resources.Caches;

public sealed class SamplerCache : ResourceCache<SamplerCache, SamplerDescriptor, ISampler>
{
	/// <inheritdoc/>
	protected override ISampler Create(SamplerDescriptor key) => Renderer.Device.ResourceFactory.CreateSampler(key);
}
