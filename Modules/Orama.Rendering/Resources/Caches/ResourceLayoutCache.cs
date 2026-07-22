// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device.Resources;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceLayoutCache : ResourceCache<ResourceLayoutCache, ResourceLayoutKey, IResourceLayout>
{
	/// <inheritdoc/>
	protected override IResourceLayout Create(ResourceLayoutKey key) => Renderer.Device.ResourceFactory.CreateResourceLayout(key);
}
