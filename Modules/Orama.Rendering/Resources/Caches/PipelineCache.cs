using NeoVeldrid;
using NeoVeldrid.SPIRV;
using System.Collections.Immutable;
using System.Xml.Linq;

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
            new CrossCompileOptions(fixClipSpaceZ: true, invertVertexOutputY: Renderer.Veldrid.GraphicsDevice.IsClipSpaceYInverted)
        );

        VertexLayoutDescription vertexLayout = new(
            new VertexElementDescription("POSITION", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("NORMAL", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("TEXCOORD0", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
        );

        ResourceLayout[] layouts = key.ResourceLayouts.Select(layoutDesc => ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(layoutDesc.Elements.ToImmutableArray())).Resource).ToArray();

        RasterizerStateDescription rasterizerState =
            Renderer.Options.Culling switch
            {
                CullingMode.None => RasterizerStateDescription.CullNone,
                CullingMode.Back => new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),
                CullingMode.Front => new RasterizerStateDescription(FaceCullMode.Front, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),

                _ => RasterizerStateDescription.CullNone
            };

        GraphicsPipelineDescription desc = new()
        {
            BlendState = BlendStateDescription.SingleOverrideBlend,
            DepthStencilState = DepthStencilStateDescription.Disabled,
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

public readonly record struct PipelineKey(string PassName, ShaderKey Shader, OutputDescription Outputs, ImmutableArray<ResourceLayoutDescription> ResourceLayouts)
{
    public bool Equals(PipelineKey other)
    {
        return PassName == other.PassName
            && Shader.VertexBytecode.SequenceEqual(other.Shader.VertexBytecode)
            && Shader.FragmentBytecode.SequenceEqual(other.Shader.FragmentBytecode)
            && Outputs.Equals(other.Outputs)
            && ResourceLayoutsEqual(ResourceLayouts, other.ResourceLayouts);
    }

    private static bool ResourceLayoutsEqual(ImmutableArray<ResourceLayoutDescription> a, ImmutableArray<ResourceLayoutDescription> b)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
            if (!ElementsEqual(a[i].Elements, b[i].Elements))
                return false;

        return true;
    }

    private static bool ElementsEqual(ResourceLayoutElementDescription[] a, ResourceLayoutElementDescription[] b)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
        {
            var x = a[i];
            var y = b[i];
            if (x.Name != y.Name || x.Kind != y.Kind || x.Stages != y.Stages || x.Options != y.Options)
                return false;
        }

        return true;
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
