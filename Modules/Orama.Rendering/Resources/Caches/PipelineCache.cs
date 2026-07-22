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

public readonly ref struct PipelineKey(string passName, ShaderKey vertShader, ShaderKey fragShader, IFramebuffer output, ReadOnlySpan<ShaderResourceGroup> resourceGroups) : IResourceKey
{
	public readonly string PassName = passName;
	public readonly ShaderKey VertShader = vertShader;
	public readonly ShaderKey FragShader = fragShader;
	public readonly IFramebuffer Output = output;
	public readonly ReadOnlySpan<ShaderResourceGroup> ResourceGroups = resourceGroups;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		hash.Add(PassName);
		hash.Add(VertShader.Hash);
		hash.Add(FragShader.Hash);
		hash.Add(Output);
		foreach (ref readonly var group in ResourceGroups)
		{
			hash.Add(group.Set);
			foreach (var element in group.LayoutElements)
			{
				hash.Add(element.Name);
				hash.Add(element.Kind);
				hash.Add(element.Stages);
			}
		}
		return hash.ToHashCode();
	}
}
