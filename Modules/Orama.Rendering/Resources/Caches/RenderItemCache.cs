using System.Collections.Immutable;
using NeoVeldrid;
using Orama.Math;

namespace Orama.Rendering.Resources.Caches;

public sealed class RenderItemCache : ResourceCache<RenderItemCache, RenderItemKey, RenderItem>
{
    /// <inheritdoc/>
    protected override RenderItem Create(RenderItemKey key)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;
        var factory = Renderer.Veldrid.GraphicsDevice.ResourceFactory;

        int vertCount = key.VertexPositions.Length;
        float[] vertexData = new float[vertCount * 8];
        for (int i = 0; i < vertCount; i++)
        {
            vertexData[i * 8 + 0] = key.VertexPositions[i].X;
            vertexData[i * 8 + 1] = key.VertexPositions[i].Y;
            vertexData[i * 8 + 2] = key.VertexPositions[i].Z;
            vertexData[i * 8 + 3] = i < key.VertexNormals.Length ? key.VertexNormals[i].X : 0f;
            vertexData[i * 8 + 4] = i < key.VertexNormals.Length ? key.VertexNormals[i].Y : 0f;
            vertexData[i * 8 + 5] = i < key.VertexNormals.Length ? key.VertexNormals[i].Z : 1f;
            vertexData[i * 8 + 6] = i < key.VertexUVs.Length ? key.VertexUVs[i].X : 0f;
            vertexData[i * 8 + 7] = i < key.VertexUVs.Length ? key.VertexUVs[i].Y : 0f;
        }

        BufferDescription vbDesc = new BufferDescription((uint)vertexData.Length * sizeof(float), BufferUsage.VertexBuffer);
        BufferDescription ibDesc = new BufferDescription((uint)key.Indices.Length * sizeof(uint), BufferUsage.IndexBuffer);

        // We dont have to get these via FrameCountedResources since RenderItem.Dispose() Disposes the vb and ib
        DeviceBuffer vb = factory.CreateBuffer(vbDesc);
        gd.UpdateBuffer(vb, 0, vertexData);

        DeviceBuffer ib = factory.CreateBuffer(ibDesc);
        gd.UpdateBuffer(ib, 0, key.Indices.AsSpan());

        FrameCountedResource<Pipeline> pipeline = PipelineCache.Instance.GetOrCreate(key.Pipeline);

        return new RenderItem(vb, ib, (uint)key.Indices.Length, pipeline.Resource);
    }
}

public readonly record struct RenderItemKey(ImmutableArray<Vector3> VertexPositions, ImmutableArray<Vector3> VertexNormals, ImmutableArray<Vector2> VertexUVs, ImmutableArray<uint> Indices, PipelineKey Pipeline)
{
    public bool Equals(RenderItemKey other)
    {
        return VertexPositions.SequenceEqual(other.VertexPositions)
            && VertexNormals.SequenceEqual(other.VertexNormals)
            && VertexUVs.SequenceEqual(other.VertexUVs)
            && Indices.SequenceEqual(other.Indices)
            && Pipeline.Equals(other.Pipeline);
    }
    
    public override int GetHashCode()
    {
        HashCode hc = new();

        foreach (var v in VertexPositions)
            hc.Add(v);

        foreach (var v in VertexNormals)
            hc.Add(v);

        foreach (var v in VertexUVs)
            hc.Add(v);

        foreach (var i in Indices)
            hc.Add(i);

        hc.Add(Pipeline);

        return hc.ToHashCode();
    }
}