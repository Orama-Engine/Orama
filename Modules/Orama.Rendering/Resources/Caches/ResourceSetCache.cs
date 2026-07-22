// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device.Resources;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceSetCache : ResourceCache<ResourceSetCache, ResourceSetKey, IResourceSet>
{
	/// <inheritdoc/>
	protected override IResourceSet Create(ResourceSetKey key) => Renderer.Device.ResourceFactory.CreateResourceSet(key);
}
