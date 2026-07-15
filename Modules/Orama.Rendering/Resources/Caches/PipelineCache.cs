// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Collections.Immutable;
using System.Xml.Linq;

using NeoVeldrid;
using NeoVeldrid.SPIRV;

namespace Orama.Rendering.Resources.Caches;

public sealed class PipelineCache : ResourceCache<PipelineCache, PipelineKey, Pipeline>
{
	/// <inheritdoc/>
	protected override Pipeline Create(PipelineKey key)
	{
		var factory = Renderer.Veldrid.GraphicsDevice.ResourceFactory;

		global::NeoVeldrid.Shader[] shaders = factory.CreateFromSpirv(
			new ShaderDescription(ShaderStages.Vertex, key.Shader.VertexBytecode, "main"),
			new ShaderDescription(ShaderStages.Fragment, key.Shader.FragmentBytecode, "main"),
			new CrossCompileOptions()
		);

		VertexLayoutDescription vertexLayout = new(
			new VertexElementDescription("POSITION", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
			new VertexElementDescription("NORMAL", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
			new VertexElementDescription("TEXCOORD0", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
		);

		ResourceLayout[] layouts = key.ResourceLayouts.Select(layoutDesc => ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(layoutDesc.Elements)).Resource).ToArray();

		RasterizerStateDescription rasterizerState =
			Renderer.Options.Culling switch
			{
				CullingMode.None => new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),
				CullingMode.Back => new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),
				CullingMode.Front => new RasterizerStateDescription(FaceCullMode.Front, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),

				_ => RasterizerStateDescription.CullNone
			};

		GraphicsPipelineDescription desc = new()
		{
			BlendState = BlendStateDescription.SingleOverrideBlend,
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

public readonly record struct PipelineKey(string PassName, ShaderKey Shader, OutputDescription Outputs, ResourceLayoutDescription[] ResourceLayouts)
{
	public bool Equals(PipelineKey other)
	{
		return PassName == other.PassName
			&& Shader.VertexBytecode.SequenceEqual(other.Shader.VertexBytecode)
			&& Shader.FragmentBytecode.SequenceEqual(other.Shader.FragmentBytecode)
			&& Outputs.Equals(other.Outputs)
			&& ResourceLayouts.SequenceEqual(other.ResourceLayouts);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		HashCode hash = new();
		hash.Add(PassName);

		foreach (byte b in Shader.VertexBytecode)
			hash.Add(b);
		foreach (byte b in Shader.FragmentBytecode)
			hash.Add(b);

		hash.Add(Outputs);

		foreach (var layout in ResourceLayouts)
			foreach (var element in layout.Elements)
			{
				hash.Add(element.Name);
				hash.Add(element.Kind);
				hash.Add(element.Stages);
				hash.Add(element.Options);
			}

		return hash.ToHashCode();
	}
}

public readonly record struct ShaderKey(byte[] VertexBytecode, byte[] FragmentBytecode);
