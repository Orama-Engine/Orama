// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device.Resources;
using Veldrith;

namespace Orama.Rendering.Resources.Caches;

public sealed class PipelineCache : ResourceCache<PipelineCache, PipelineKey, Pipeline>
{
	/// <inheritdoc/>
	protected override Pipeline Create(PipelineKey key)
	{
		var factory = Renderer.Device.ResourceFactory;

#if DEBUG
		bool shouldDebug = true;
#else
		bool shouldDebug = false;
#endif

		IShader[] shaders = [factory.CreateShader(key.VertShader), factory.CreateShader(key.FragShader)];

		VertexLayoutDescription vertexLayout = new(
			new VertexElementDescription("POSITION", VertexElementSemantic.Position, VertexElementFormat.Float3),
			new VertexElementDescription("NORMAL", VertexElementSemantic.Normal, VertexElementFormat.Float3),
			new VertexElementDescription("TEXCOORD0", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
		);

		var layouts = new ResourceLayout[key.ResourceLayouts.Length];
		for (int i = 0; i < key.ResourceLayouts.Length; i++)
		{
			var layoutDesc = key.ResourceLayouts[i];
			layouts[i] = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(layoutDesc.Elements)).Resource;
		}

		RasterizerStateDescription rasterizerState =
			Renderer.Options.Culling switch
			{
				CullingMode.None => new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),
				CullingMode.Back => new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),
				CullingMode.Front => new RasterizerStateDescription(FaceCullMode.Front, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),

				_ => RasterizerStateDescription.CULL_NONE
			};

		GraphicsPipelineDescription desc = new()
		{
			BlendState = BlendStateDescription.SINGLE_OVERRIDE_BLEND,
			DepthStencilState = new DepthStencilStateDescription(
				depthTestEnabled: true,
				depthWriteEnabled: true,
				comparisonKind: ComparisonKind.LessEqual
			),
			RasterizerState = rasterizerState,
			PrimitiveTopology = PrimitiveTopology.TriangleList,
			ShaderSet = new ShaderSetDescription(new[] { vertexLayout }, shaders),
			ResourceLayouts = layouts,
			Outputs = key.Outputs,
		};

		Pipeline pipeline = factory.CreateGraphicsPipeline(ref desc);

		return pipeline;
	}
}

public readonly ref struct PipelineKey(string passName, ShaderKey vertShader, ShaderKey fragShader, OutputDescription outputs, ReadOnlySpan<ResourceLayoutDescription> resourceLayouts) : IResourceKey
{
	public readonly string PassName = passName;
	public readonly ShaderKey VertShader = vertShader;
	public readonly ShaderKey FragShader = fragShader;
	public readonly OutputDescription Outputs = outputs;

	public readonly ReadOnlySpan<ResourceLayoutDescription> ResourceLayouts = resourceLayouts;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(PipelineKey other)
	{
		if (PassName != other.PassName)
			return false;
		if (!Outputs.Equals(other.Outputs))
			return false;

		if (!VertShader.Bytecode.SequenceEqual(other.VertShader.Bytecode))
			return false;

		if (!FragShader.Bytecode.SequenceEqual(other.FragShader.Bytecode))
			return false;

		if (ResourceLayouts.Length != other.ResourceLayouts.Length)
			return false;

		for (int i = 0; i < ResourceLayouts.Length; i++)
		{
			var leftLayout = ResourceLayouts[i];
			var rightLayout = other.ResourceLayouts[i];

			if (leftLayout.Elements.Length != rightLayout.Elements.Length)
				return false;

			for (int j = 0; j < leftLayout.Elements.Length; j++)
			{
				if (!leftLayout.Elements[j].Equals(rightLayout.Elements[j]))
					return false;
			}
		}

		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;

			hash = hash * 31 + (PassName?.GetHashCode() ?? 0);

			hash = hash * 31 + VertShader.Hash;
			hash = hash * 31 + FragShader.Hash;

			hash = hash * 31 + Outputs.GetHashCode();

			foreach (var layout in ResourceLayouts)
			{
				if (layout.Elements == null)
					continue;

				foreach (var element in layout.Elements)
				{
					hash = hash * 31 + (element.Name?.GetHashCode() ?? 0);
					hash = hash * 31 + (int)element.Kind;
					hash = hash * 31 + (int)element.Stages;
					hash = hash * 31 + (int)element.Options;
				}
			}

			return hash;
		}
	}
}
