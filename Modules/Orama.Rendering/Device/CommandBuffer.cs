using Orama.Math;
using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;
using Orama.Rendering.Veldrid;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using NeoVeldrid;

namespace Orama.Rendering.Device;

/// <summary>
/// A Buffer of GPU commands to be submitted.
/// </summary>
public class CommandBuffer : IDisposable
{
    /// <summary> The low-level Veldrid command list. </summary>
    public CommandList CommandList { get; }

    /// <summary> The current pipeline in use. </summary>
    public PipelineKey Pipeline { get; set; }

    /// <summary> Initializes a new instance of the <see cref="CommandBuffer"/> class. </summary>
    public CommandBuffer(VeldridDevice device) => CommandList = device.GraphicsDevice.ResourceFactory.CreateCommandList();

    private static Dictionary<uint, GPUBuffer> gpuBufferQueue = new Dictionary<uint, GPUBuffer>();

    /// <inheritdoc/>
    public void Dispose() => CommandList.Dispose();

    public void Begin() => CommandList.Begin();

    public void End() => CommandList.End();

    public void ClearColor(Color color) => CommandList.ClearColorTarget(0, new global::NeoVeldrid.RgbaFloat(color.R, color.G, color.B, color.A));

    public void QueueGPUBuffer(GPUBuffer gpuBuffer, uint slot) => gpuBufferQueue[slot] = gpuBuffer;

    public void DrawRenderable(IClientRenderable renderable)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;

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

        // index 0: MaterialParams
        // index 1: ObjectParams
        ResourceLayoutDescription layoutDesc = new(new ResourceLayoutElementDescription("MaterialParams", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment));

        PipelineKey pipelineDesc = new PipelineKey(
            PassName: renderable.Material.Pass,
            Shader: new ShaderKey(renderable.Material.Shader.VertexBytecode, renderable.Material.Shader.FragmentBytecode),
            Outputs: gd.SwapchainFramebuffer.OutputDescription,
            ResourceLayout: layoutDesc
        );

        var vertexBytes = MemoryMarshal.AsBytes(vertexData.AsSpan()).ToArray();
        var indexBytes = MemoryMarshal.AsBytes(renderable.Indices.AsSpan()).ToArray();

        FrameCountedResource<RenderItem> item = RenderItemCache.Instance.GetOrCreate(new RenderItemKey(
            VertexBuffer: new RIBufferKey(ImmutableCollectionsMarshal.AsImmutableArray(vertexBytes), BufferUsage.VertexBuffer),
            IndexBuffer: new RIBufferKey(ImmutableCollectionsMarshal.AsImmutableArray(indexBytes), BufferUsage.IndexBuffer),
            IndexCount: (uint)renderable.Indices.Length,
            Pipeline: pipelineDesc
        ));

        SetPipeline(pipelineDesc);

        foreach (var (slot, gpuBuffer) in gpuBufferQueue)
            UploadUniformBuffer(Pipeline, (uint)gpuBuffer.Data.Length, slot, gpuBuffer.Data);

        gpuBufferQueue.Clear();

        DrawItem(item.Resource);
    }

    public void UploadUniformBuffer(PipelineKey target, uint size, uint index, ReadOnlySpan<byte> data)
    {
        FrameCountedResource<DeviceBuffer> buffer = DeviceBufferCache.Instance.GetOrCreate(new DeviceBufferKey(size, BufferUsage.UniformBuffer));
        CommandList.UpdateBuffer(buffer.Resource, 0, data.ToArray());

        FrameCountedResource<ResourceLayout> layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(target.ResourceLayout.Elements.ToImmutableArray()));

        FrameCountedResource<ResourceSet> resourceSet = ResourceSetCache.Instance.GetOrCreate(new ResourceSetKey(layout.Resource, buffer.Resource));
        CommandList.SetGraphicsResourceSet(index, resourceSet.Resource);
    }

    public void SetPipeline(PipelineKey pipelineDesc)
    {
        Pipeline = pipelineDesc;

        FrameCountedResource<Pipeline> pipeline = PipelineCache.Instance.GetOrCreate(pipelineDesc);

        CommandList.SetPipeline(pipeline.Resource);
    }

    public void DrawItem(RenderItem item)
    {
        CommandList.SetVertexBuffer(0, item.VertexBuffer);
        CommandList.SetIndexBuffer(item.IndexBuffer, IndexFormat.UInt32);
        CommandList.DrawIndexed(item.IndexCount);
    }
}
