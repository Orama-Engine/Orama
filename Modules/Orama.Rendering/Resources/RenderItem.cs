
using System.Numerics;
using Veldrid;

namespace Orama.Rendering.Resources;

/// <summary>
/// Low-Level representation of a rendered object.
/// </summary>
public class RenderItem : IDisposable
{
    public DeviceBuffer VertexBuffer { get; }
    public DeviceBuffer IndexBuffer { get; }
    public uint IndexCount { get; }
    public Pipeline Pipeline { get; }

    public RenderItem(DeviceBuffer vertexBuffer, DeviceBuffer indexBuffer, uint indexCount, Pipeline pipeline)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        IndexCount = indexCount;
        Pipeline = pipeline;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}
