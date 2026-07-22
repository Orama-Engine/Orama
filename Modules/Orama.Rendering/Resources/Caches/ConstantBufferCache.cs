// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)


using Orama.Rendering.Device.Resources;

namespace Orama.Rendering.Resources.Caches;

public sealed class ConstantBufferCache : ResourceCache<ConstantBufferCache, ConstantBufferDescriptor, IBuffer>
{
	/// <inheritdoc/>
	protected override IBuffer Create(ConstantBufferDescriptor key) => Renderer.Device.ResourceFactory.CreateBuffer(new BufferDescriptor(key.Size, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
}
