// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device.Resources;

namespace Orama.Rendering.Resources.Caches;

/// <summary>
/// Caches graphics pipelines.
/// </summary>
public sealed class PipelineCache : ResourceCache<PipelineCache, PipelineKey, IPipeline>
{
	/// <inheritdoc/>
	protected override IPipeline Create(PipelineKey key) => Renderer.Device.ResourceFactory.CreateGraphicsPipeline(key);
}
