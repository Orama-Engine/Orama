// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Pipelines.Forward;

public class ForwardRenderPipeline : RenderPipeline
{
	/// <inheritdoc/>
	public override RenderPass[] Passes { get; } = [new OpaquePass()];
}
