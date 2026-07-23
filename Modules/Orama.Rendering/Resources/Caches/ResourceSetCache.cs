// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.RHI.Resources;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceSetCache : ResourceCache<ResourceSetCache, ResourceDescriptor, IResourceSet>
{
	/// <inheritdoc/>
	protected override IResourceSet Create(ResourceDescriptor key) => Renderer.Device.ResourceFactory.CreateResourceSet(key);
}
