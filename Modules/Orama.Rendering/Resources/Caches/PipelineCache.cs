// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.RHI.Resources;

namespace Orama.Rendering.Resources.Caches;

/// <summary>
/// Caches graphics pipelines.
/// </summary>
public sealed class PipelineCache : ResourceCache<PipelineCache, PipelineDescriptor, IPipeline>
{
	/// <inheritdoc/>
	protected override IPipeline Create(PipelineDescriptor key) => Renderer.Device.ResourceFactory.CreateGraphicsPipeline(key);
}
