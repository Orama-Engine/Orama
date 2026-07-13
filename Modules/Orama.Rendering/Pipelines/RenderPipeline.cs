// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Pipelines;

/// <summary>
/// A Collection of render passes.
/// </summary>
public abstract class RenderPipeline
{
	/// <summary> The render passes used by the pipeline. </summary>
	public abstract RenderPass[] Passes { get; }

	/// <summary> The <see cref="IShaderDefaultsProvider"/> used by the pipeline. </summary>
	/// <remarks> Defaults to <see cref="Rendering.ShaderDefaultsProvider"/>. </remarks>
	public virtual IShaderDefaultsProvider ShaderDefaultsProvider { get; } = new ShaderDefaultsProvider();

	/// <summary> Renders the pipeline. </summary>
	public void Render(in RenderFrame frame)
	{
		foreach (var pass in Passes)
			pass.Execute(in frame);
	}
}
