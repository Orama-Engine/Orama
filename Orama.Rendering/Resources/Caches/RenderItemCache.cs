using System;
using System.Collections.Generic;
using System.Text;
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
        gd.UpdateBuffer(vb, 0, key.VertexBuffer.Data);

        DeviceBuffer ib = factory.CreateBuffer(ibDesc);
        gd.UpdateBuffer(ib, 0, key.IndexBuffer.Data);

        Pipeline pipeline = PipelineCache.Instance.GetOrCreate(key.Pipeline);

        return new RenderItem(vb, ib, key.IndexCount, pipeline);
    }
}

public readonly record struct BufferDescriptor(byte[] Data, BufferUsage Usage)
{
    public bool Equals(BufferDescriptor other) => Usage == other.Usage && Data.AsSpan().SequenceEqual(other.Data);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        // This is very expensive
        // TODO: Better comparison

        var hash = new HashCode();
        hash.Add(Usage);
        foreach (var b in Data)
            hash.Add(b);

        return hash.ToHashCode();
    }
}

public readonly record struct RenderItemKey(BufferDescriptor VertexBuffer, BufferDescriptor IndexBuffer, uint IndexCount, PipelineKey Pipeline);