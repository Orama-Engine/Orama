
using Orama.Rendering.Resources;
using Orama.Rendering.Veldrid;
using Silk.NET.GLFW;
using Veldrid;

namespace Orama.Rendering.Device;

public class CommandBuffer : IDisposable
{
    /// <summary> The low-level Veldrid command list. </summary>
    public CommandList CommandList { get; }

    /// <summary> Initializes a new instance of the <see cref="CommandBuffer"/> class. </summary>
    public CommandBuffer(VeldridDevice device) => CommandList = device.GraphicsDevice.ResourceFactory.CreateCommandList();

    /// <inheritdoc/>
    public void Dispose() => CommandList.Dispose();

    public void Begin() => CommandList.Begin();

    public void End() => CommandList.End();

    public void UploadUniformBuffer(PipelineDescriptor target, uint size, uint index, ReadOnlySpan<byte> data)
    {
        DeviceBuffer buffer = Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription(size, BufferUsage.UniformBuffer));
        CommandList.UpdateBuffer(buffer, 0, data.ToArray());

        ResourceLayout layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutDescriptor(target.ResourceLayout.Elements));

        ResourceSet resourceSet = Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(layout, buffer));
        CommandList.SetGraphicsResourceSet(index, resourceSet);
    }

    public void SetPipeline(Pipeline pipeline)
    {
        CommandList.SetPipeline(pipeline);
    }

    public void DrawItem(RenderItem item)
    {
        CommandList.SetVertexBuffer(0, item.VertexBuffer);
        CommandList.SetIndexBuffer(item.IndexBuffer, IndexFormat.UInt32);
        CommandList.DrawIndexed(item.IndexCount);
    }
}
