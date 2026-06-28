using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;
using Orama.Rendering.Veldrid;
using System.Collections.Immutable;
using Veldrid;

namespace Orama.Rendering.Device;

public class CommandBuffer : IDisposable
{
    /// <summary> The low-level Veldrid command list. </summary>
    public CommandList CommandList { get; }

    public PipelineKey Pipeline { get; set; }

    /// <summary> Initializes a new instance of the <see cref="CommandBuffer"/> class. </summary>
    public CommandBuffer(VeldridDevice device) => CommandList = device.GraphicsDevice.ResourceFactory.CreateCommandList();

    /// <inheritdoc/>
    public void Dispose() => CommandList.Dispose();

    public void Begin() => CommandList.Begin();

    public void End() => CommandList.End();

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
