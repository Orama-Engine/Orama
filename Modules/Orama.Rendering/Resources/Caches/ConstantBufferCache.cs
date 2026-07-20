// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)


using Veldrith;

namespace Orama.Rendering.Resources.Caches;

public sealed class ConstantBufferCache : ResourceCache<ConstantBufferCache, ConstantBufferKey, DeviceBuffer>
{
	/// <inheritdoc/>
	protected override DeviceBuffer Create(ConstantBufferKey key)
	{
		var gd = Renderer.Veldrith.GraphicsDevice;
		var factory = gd.ResourceFactory;
		
		// Should we align size here or leave it up to the user/GPUBuffer?
		uint alignedSize = (uint)((key.Data.Length + 15) & ~15);

		var desc = new BufferDescription(alignedSize, BufferUsage.UniformBuffer | BufferUsage.Dynamic);
		DeviceBuffer buffer = factory.CreateBuffer(desc);

		gd.UpdateBuffer(buffer, 0, key.Data);

		return buffer;
	}
}

public readonly ref struct ConstantBufferKey(string name, ReadOnlySpan<byte> data) : IResourceKey
{
	public readonly ReadOnlySpan<byte> Data = data;

	public ReadOnlySpan<char> Name => name;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(ConstantBufferKey other)
	{
		if (Data.Length != other.Data.Length)
			return false;

		if (!Name.SequenceEqual(other.Name))
			return false;

		return Data.SequenceEqual(other.Data);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			foreach (byte b in Data)
				hash = hash * 31 + b;

			hash = hash * 31 + name.GetHashCode();
			return hash;
		}
	}
}
