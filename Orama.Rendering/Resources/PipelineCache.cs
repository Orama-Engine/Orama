using Veldrid;
using Veldrid.SPIRV;

namespace Orama.Rendering.Resources;

public sealed class PipelineCache : ResourceCache<PipelineCache, PipelineDescription, Pipeline>
{
    /// <inheritdoc/>
    protected override Pipeline Create(PipelineDescription key)
    {
        var factory = Renderer.Veldrid.GraphicsDevice.ResourceFactory;

        byte[] vertBytes = File.ReadAllBytes($"shaders/{key.PassName}.vert.spv");
        byte[] fragBytes = File.ReadAllBytes($"shaders/{key.PassName}.frag.spv");

        Shader[] shaders = factory.CreateFromSpirv(
            new ShaderDescription(ShaderStages.Vertex, vertBytes, "main"),
            new ShaderDescription(ShaderStages.Fragment, fragBytes, "main")
        );

        VertexLayoutDescription vertexLayout = new(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("UV", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
        );

        GraphicsPipelineDescription desc = new()
        {
            BlendState = BlendStateDescription.SingleOverrideBlend,
            DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
            RasterizerState = RasterizerStateDescription.Default,
            PrimitiveTopology = PrimitiveTopology.TriangleList,
            ShaderSet = new ShaderSetDescription(new[] { vertexLayout }, shaders),
            ResourceLayouts = key.ResourceLayouts,
            Outputs = key.Outputs,
        };

        return factory.CreateGraphicsPipeline(desc);
    }
}

public readonly record struct PipelineDescription(string PassName, OutputDescription Outputs, ResourceLayout[] ResourceLayouts);