// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System;
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

		DeviceBuffer vb = factory.CreateBuffer(vbDesc);
		gd.UpdateBuffer(vb, 0, vertexData);

		DeviceBuffer ib = factory.CreateBuffer(ibDesc);
		gd.UpdateBuffer(ib, 0, key.Indices);

		FrameCountedResource<Pipeline> pipeline = PipelineCache.Instance.GetOrCreate(key.Pipeline);

		return new RenderItem(vb, ib, (uint)key.Indices.Length, pipeline.Resource);
	}
}

public readonly ref struct RenderItemKey(ReadOnlySpan<Vector3> vertexPositions, ReadOnlySpan<Vector3> vertexNormals, ReadOnlySpan<Vector2> vertexUVs, ReadOnlySpan<uint> indices, PipelineKey pipeline) : IResourceKey
{
	public readonly ReadOnlySpan<Vector3> VertexPositions = vertexPositions;
	public readonly ReadOnlySpan<Vector3> VertexNormals = vertexNormals;
	public readonly ReadOnlySpan<Vector2> VertexUVs = vertexUVs;
	public readonly ReadOnlySpan<uint> Indices = indices;
	public readonly PipelineKey Pipeline = pipeline;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(RenderItemKey other)
	{
		if (!VertexPositions.SequenceEqual(other.VertexPositions)) return false;
		if (!VertexNormals.SequenceEqual(other.VertexNormals)) return false;
		if (!VertexUVs.SequenceEqual(other.VertexUVs)) return false;
		if (!Indices.SequenceEqual(other.Indices)) return false;
		if (!Pipeline.Equals(other.Pipeline)) return false;

		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;

			foreach (var v in VertexPositions) hash = hash * 31 + v.GetHashCode();
			foreach (var v in VertexNormals) hash = hash * 31 + v.GetHashCode();
			foreach (var v in VertexUVs) hash = hash * 31 + v.GetHashCode();
			foreach (var i in Indices) hash = hash * 31 + (int)i;

			hash = hash * 31 + Pipeline.Hash;
			return hash;
		}
	}
}
