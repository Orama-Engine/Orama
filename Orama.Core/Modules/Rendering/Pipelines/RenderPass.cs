
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
    public abstract void Render();

    /// <summary> Queues a renderable object to be rendered during the next frame. </summary>
    protected void DrawObject(IClientRenderable renderable, CommandBuffer buffer)
    {
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

        DeviceBuffer vb = factory.CreateBuffer(new BufferDescription((uint)(vertexData.Length * sizeof(float)), BufferUsage.VertexBuffer));
        gd.UpdateBuffer(vb, 0, vertexData);

        DeviceBuffer ib = factory.CreateBuffer(new BufferDescription((uint)(renderable.Indices.Length * sizeof(uint)), BufferUsage.IndexBuffer));
        gd.UpdateBuffer(ib, 0, renderable.Indices);

        var shaderDescriptor = new ShaderDescriptor(
            VertexSource: renderable.Material.Shader.Vertex,
            FragmentSource: renderable.Material.Shader.Fragment
        );

        var descriptor = new PipelineDescriptor(
            PassName: renderable.Material.Pass,
            Shader: shaderDescriptor,
            Outputs: gd.SwapchainFramebuffer.OutputDescription,
            ResourceLayouts: Array.Empty<ResourceLayout>()
        );

        Pipeline pipeline = PipelineCache.Instance.GetOrCreate(descriptor);

        RenderItem item = new(vb, ib, (uint)renderable.Indices.Length, pipeline, Array.Empty<ResourceSet>());

        buffer.CommandList.SetPipeline(item.Pipeline);
        buffer.CommandList.SetVertexBuffer(0, item.VertexBuffer);
        buffer.CommandList.SetIndexBuffer(item.IndexBuffer, IndexFormat.UInt32);
        buffer.CommandList.DrawIndexed(item.IndexCount);
    }
}
