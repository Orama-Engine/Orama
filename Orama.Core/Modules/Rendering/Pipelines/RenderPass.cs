
using Orama.Rendering;
using Orama.Rendering.Device;
using Orama.Rendering.Resources;
using Veldrid;
using Vulkan;

namespace Orama.Core.Modules.Rendering.Pipelines;

/// <summary>
/// Render instructions for a step in the rendering pipeline.
/// </summary>
public abstract class RenderPass
{
    protected CommandBuffer CommandBuffer { get; set; } = null!;

    public void BeginFrame(CommandBuffer buffer)
    {
        CommandBuffer = buffer;
    }

    public abstract void Render();

    /// <summary> Queues a renderable object to be rendered during the next frame. </summary>
    public void QueueObject(IClientRenderable renderable)
    {
        // TODO: Instantiating a new RenderItem multiple times every frame is very expensive, don't do this
        var gd = Renderer.Veldrid.GraphicsDevice;
        var factory = gd.ResourceFactory;

        int vertCount = renderable.Vertices.Length;
        float[] vertexData = new float[vertCount * 8];
        for (int i = 0; i < vertCount; i++)
        {
            vertexData[i * 8 + 0] = renderable.Vertices[i].X;
            vertexData[i * 8 + 1] = renderable.Vertices[i].Y;
            vertexData[i * 8 + 2] = renderable.Vertices[i].Z;
            vertexData[i * 8 + 3] = i < renderable.Normals.Length ? renderable.Normals[i].X : 0f;
            vertexData[i * 8 + 4] = i < renderable.Normals.Length ? renderable.Normals[i].Y : 0f;
            vertexData[i * 8 + 5] = i < renderable.Normals.Length ? renderable.Normals[i].Z : 1f;
            vertexData[i * 8 + 6] = i < renderable.UVs.Length ? renderable.UVs[i].X : 0f;
            vertexData[i * 8 + 7] = i < renderable.UVs.Length ? renderable.UVs[i].Y : 0f;
        }

        DeviceBuffer vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)(vertexData.Length * sizeof(float)), BufferUsage.VertexBuffer));
        gd.UpdateBuffer(vertexBuffer, 0, vertexData);

        DeviceBuffer indexBuffer = factory.CreateBuffer(new BufferDescription((uint)(renderable.Indices.Length * sizeof(uint)), BufferUsage.IndexBuffer));
        gd.UpdateBuffer(indexBuffer, 0, renderable.Indices);

        var descriptor = new PipelineDescription(
            PassName: renderable.Material.Pass,
            Outputs: gd.SwapchainFramebuffer.OutputDescription,
            ResourceLayouts: Array.Empty<ResourceLayout>()
        );

        Pipeline pipeline = PipelineCache.Instance.GetOrCreate(descriptor);

        RenderItem item = new(
            vertexBuffer,
            indexBuffer,
            (uint)renderable.Indices.Length,
            pipeline,
            Array.Empty<ResourceSet>()
        );

        CommandBuffer.CommandList.SetPipeline(item.Pipeline);
        CommandBuffer.CommandList.SetVertexBuffer(0, item.VertexBuffer);
        CommandBuffer.CommandList.SetIndexBuffer(item.IndexBuffer, IndexFormat.UInt32);
        CommandBuffer.CommandList.DrawIndexed(item.IndexCount);

        Renderer.QueueMesh(item);
    }
}
