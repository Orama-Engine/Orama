
using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;
using Orama.Rendering.Veldrid;
using Silk.NET.GLFW;
using System.Collections.Immutable;
using Veldrid;

namespace Orama.Rendering.Device;

public class CommandBuffer : IDisposable
{
    /// <summary> The low-level Veldrid command list. </summary>
    public CommandList CommandList { get; }

    public PipelineKey Pipeline { get; set; }

    private readonly List<(RCGHandle<Pipeline> Handle, ulong Frame)> acquiredPipelines = new();

    /// <summary> Initializes a new instance of the <see cref="CommandBuffer"/> class. </summary>
    public CommandBuffer(VeldridDevice device) => CommandList = device.GraphicsDevice.ResourceFactory.CreateCommandList();

    /// <inheritdoc/>
    public void Dispose() => CommandList.Dispose();

    public void Begin() => CommandList.Begin();

    public void End()
    {
        CommandList.End();

        foreach (var (handle, frame) in acquiredPipelines)
            RCGResourceRegistry<Pipeline>.Instance.Release(handle, frame);

        acquiredPipelines.Clear();
    }

    public void UploadUniformBuffer(PipelineKey target, uint size, uint index, ReadOnlySpan<byte> data)
    {
        DeviceBuffer buffer = Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription(size, BufferUsage.UniformBuffer));
        CommandList.UpdateBuffer(buffer, 0, data.ToArray());

        ResourceLayout layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(target.ResourceLayout.Elements.ToImmutableArray()));

        ResourceSet resourceSet = Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(layout, buffer));
        CommandList.SetGraphicsResourceSet(index, resourceSet);
    }

    public void SetPipeline(PipelineKey pipelineDesc)
    {
        Pipeline = pipelineDesc;

        Pipeline pipeline = PipelineCache.Instance.GetOrCreate(pipelineDesc);
        CommandList.SetPipeline(pipeline);
    }

    public void DrawItem(RenderItem item)
    {
        CommandList.SetVertexBuffer(0, item.VertexBuffer);
        CommandList.SetIndexBuffer(item.IndexBuffer, IndexFormat.UInt32);
        CommandList.DrawIndexed(item.IndexCount);
    }
}
