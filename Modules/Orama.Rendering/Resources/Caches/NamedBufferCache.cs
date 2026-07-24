// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)


using Orama.RHI.Resources;

namespace Orama.Rendering.Resources.Caches;

public sealed class NamedBufferCache : ResourceCache<NamedBufferCache, NamedBufferDescriptor, IBuffer>
{
	/// <inheritdoc/>
	protected override IBuffer Create(NamedBufferDescriptor key) => Renderer.Device.ResourceFactory.CreateBuffer(new BufferDescriptor(key.Size, key.Usage));
}
