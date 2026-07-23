// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.RHI.Resources;

namespace Orama.Rendering.Resources.Caches;

public sealed class ResourceLayoutCache : ResourceCache<ResourceLayoutCache, ResourceLayoutDescriptor, IResourceLayout>
{
	/// <inheritdoc/>
	protected override IResourceLayout Create(ResourceLayoutDescriptor key) => Renderer.Device.ResourceFactory.CreateResourceLayout(key);
}
