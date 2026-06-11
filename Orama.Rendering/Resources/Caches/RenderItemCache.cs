using System.Collections.Immutable;
using Veldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class RenderItemCache : ResourceCache<RenderItemCache, RenderItemKey, RenderItem>
{
    /// <inheritdoc/>
    protected override RenderItem Create(RenderItemKey key)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;
        var factory = Renderer.Veldrid.GraphicsDevice.ResourceFactory;

        BufferDescription vbDesc = new BufferDescription((uint)key.VertexBuffer.Data.Length, key.VertexBuffer.Usage);
        BufferDescription ibDesc = new BufferDescription((uint)key.IndexBuffer.Data.Length, key.IndexBuffer.Usage);

        DeviceBuffer vb = factory.CreateBuffer(vbDesc);
        gd.UpdateBuffer(vb, 0, key.VertexBuffer.Data.AsSpan());

        DeviceBuffer ib = factory.CreateBuffer(ibDesc);
        gd.UpdateBuffer(ib, 0, key.IndexBuffer.Data.AsSpan());

        FrameCountedResource<Pipeline> pipeline = PipelineCache.Instance.GetOrCreate(key.Pipeline);

        return new RenderItem(vb, ib, key.IndexCount, pipeline.Resource);
    }
}

public readonly record struct BufferDescriptor(ImmutableArray<byte> Data, BufferUsage Usage)
{
    private readonly int hash = ComputeHash(Data, Usage);

    private static int ComputeHash(ImmutableArray<byte> data, BufferUsage usage)
    {
        var hash = new HashCode();
        hash.Add(usage);
        hash.AddBytes(data.AsSpan());
        return hash.ToHashCode();
    }

    /// <inheritdoc/>
    public override int GetHashCode() => hash;

    /// <inheritdoc/>
    public bool Equals(BufferDescriptor other) => Usage == other.Usage && Data.AsSpan().SequenceEqual(other.Data.AsSpan());

}

public readonly record struct RenderItemKey(BufferDescriptor VertexBuffer, BufferDescriptor IndexBuffer, uint IndexCount, PipelineKey Pipeline);