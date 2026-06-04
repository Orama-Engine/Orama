using System.Collections.Immutable;
using Veldrid;
using Veldrid.SPIRV;
using Vulkan;

namespace Orama.Rendering.Resources.Caches;

public sealed class PipelineCache : ResourceCache<PipelineCache, PipelineKey, Pipeline>
{
    /// <inheritdoc/>
    protected override Pipeline Create(PipelineKey key)
    {
        var factory = Renderer.Veldrid.GraphicsDevice.ResourceFactory;

        Shader[] shaders = factory.CreateFromSpirv(
            new ShaderDescription(ShaderStages.Vertex, key.Shader.VertexBytecode, "main"),
            new ShaderDescription(ShaderStages.Fragment, key.Shader.FragmentBytecode, "main")
        );

        VertexLayoutDescription vertexLayout = new(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("UV", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
        );

        FrameCountedResource<ResourceLayout> layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(key.ResourceLayout.Elements.ToImmutableArray()));

        GraphicsPipelineDescription desc = new()
        {
            BlendState = BlendStateDescription.SingleOverrideBlend,
            DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
            RasterizerState = RasterizerStateDescription.CullNone,
            PrimitiveTopology = PrimitiveTopology.TriangleList,
            ShaderSet = new ShaderSetDescription(new[] { vertexLayout }, shaders),
            ResourceLayouts = new[] { layout.Resource },
            Outputs = key.Outputs,
        };

        Pipeline pipeline = factory.CreateGraphicsPipeline(desc);

        return pipeline;
    }
}

public readonly record struct PipelineKey(string PassName, ShaderKey Shader, OutputDescription Outputs, ResourceLayoutDescription ResourceLayout);

public readonly record struct ShaderKey(byte[] VertexBytecode, byte[] FragmentBytecode);