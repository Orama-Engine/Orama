
using Orama.Rendering.Resources;
using Orama.Rendering.Veldrid;
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

    public void DrawItem(RenderItem item)
    {
        CommandList.SetPipeline(item.Pipeline);
        CommandList.SetVertexBuffer(0, item.VertexBuffer);
        CommandList.SetIndexBuffer(item.IndexBuffer, IndexFormat.UInt32);
        CommandList.DrawIndexed(item.IndexCount);
    }
}
