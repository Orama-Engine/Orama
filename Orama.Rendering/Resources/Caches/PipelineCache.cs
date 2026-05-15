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

        (byte[] vertBytes, byte[] fragBytes) = ShaderBaker.HLSLToShader(key.Shader.VertexSource, key.Shader.FragmentSource);

        Shader[] shaders = factory.CreateFromSpirv(
            new ShaderDescription(ShaderStages.Vertex, vertBytes, "main"),
            new ShaderDescription(ShaderStages.Fragment, fragBytes, "main")
        );

        VertexLayoutDescription vertexLayout = new(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("UV", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
        );

        ResourceLayout layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(key.ResourceLayout.Elements.ToImmutableArray()));

        GraphicsPipelineDescription desc = new()
        {
            BlendState = BlendStateDescription.SingleOverrideBlend,
            DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
            RasterizerState = RasterizerStateDescription.CullNone,
            PrimitiveTopology = PrimitiveTopology.TriangleList,
            ShaderSet = new ShaderSetDescription(new[] { vertexLayout }, shaders),
            ResourceLayouts = new[] { layout },
            Outputs = key.Outputs,
        };

        Pipeline pipeline = factory.CreateGraphicsPipeline(desc);

        return pipeline;
    }
}

public readonly record struct PipelineKey(string PassName, ShaderKey Shader, OutputDescription Outputs, ResourceLayoutDescription ResourceLayout);

public readonly record struct ShaderKey(string VertexSource, string FragmentSource);