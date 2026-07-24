// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Math;
using Orama.RHI;

namespace Orama.Rendering.Pipelines.Forward;

/// <summary>
/// Pass responsible for rendering all solid objects.
/// </summary>
public class OpaquePass : RenderPass
{
	/// <inheritdoc/>
	public override void Render(in RenderFrame frame, ICommandBuffer buffer)
	{
		buffer.ClearDepth(1.0f);
		buffer.ClearColor(Color.Black);

		foreach (IClientRenderable renderable in frame.Renderables)
		{
			if (renderable.Material.Shader.Pass == "Opaque")
				buffer.Draw(renderable);
		}
	}
}
