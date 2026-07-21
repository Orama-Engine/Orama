// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Veldrith;
using Veldrith.SPIRV;

namespace Orama.Rendering.Resources.Caches;

public sealed class PipelineCache : ResourceCache<PipelineCache, PipelineKey, Pipeline>
{
	/// <inheritdoc/>
	protected override Pipeline Create(PipelineKey key)
	{
		var factory = Renderer.Device.GraphicsDevice.ResourceFactory;

#if DEBUG
		bool shouldDebug = true;
#else
		bool shouldDebug = false;
#endif

		global::Veldrith.Shader[] shaders = factory.CreateFromSpirv(
			new ShaderDescription(ShaderStages.Vertex, key.Shader.VertexBytecode.ToArray(), "main", debug: shouldDebug),
			new ShaderDescription(ShaderStages.Fragment, key.Shader.FragmentBytecode.ToArray(), "main", debug: shouldDebug),
			new CrossCompileOptions(fixClipSpaceZ: true, invertVertexOutputY: true, normalizeResourceNames: false)
		);

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

public readonly ref struct PipelineKey(string passName, ShaderKey shader, OutputDescription outputs, ReadOnlySpan<ResourceLayoutDescription> resourceLayouts) : IResourceKey
{
	public readonly string PassName = passName;
	public readonly ShaderKey Shader = shader;
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

		if (!Shader.VertexBytecode.SequenceEqual(other.Shader.VertexBytecode))
			return false;
		if (!Shader.FragmentBytecode.SequenceEqual(other.Shader.FragmentBytecode))
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

			hash = hash * 31 + Shader.Hash;

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

public readonly ref struct ShaderKey(ReadOnlySpan<byte> vertexBytecode, ReadOnlySpan<byte> fragmentBytecode) : IResourceKey
{
	public readonly ReadOnlySpan<byte> VertexBytecode = vertexBytecode;
	public readonly ReadOnlySpan<byte> FragmentBytecode = fragmentBytecode;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;

			foreach (byte b in VertexBytecode)
				hash = hash * 31 + b;

			foreach (byte b in FragmentBytecode)
				hash = hash * 31 + b;

			return hash;
		}
	}
}
