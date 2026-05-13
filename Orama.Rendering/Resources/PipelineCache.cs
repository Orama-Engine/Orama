using Veldrid;
using Veldrid.SPIRV;

namespace Orama.Rendering.Resources;

public sealed class PipelineCache : ResourceCache<PipelineCache, PipelineDescription, Pipeline>
{
    const string VERT_SOURCE = @"
#version 450

vec2 positions[3] = vec2[](
    vec2( 0.0,  0.2),
    vec2(-0.2, -0.2),
    vec2( 0.2, -0.2)
);

void main()
{
    gl_Position = vec4(positions[gl_VertexIndex], 0.0, 1.0);
}
";

    const string FRAG_SOURCE = @"
#version 450

layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0, 0.0, 1.0, 1.0);
}
";

    /// <inheritdoc/>
    protected override Pipeline Create(PipelineDescription key)
    {
        var factory = Renderer.Veldrid.GraphicsDevice.ResourceFactory;

        (byte[] vertBytes, byte[] fragBytes) = ShaderBaker.GLSLToShader(VERT_SOURCE, FRAG_SOURCE);

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