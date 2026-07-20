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

		var desc = new BufferDescription(key.Size, BufferUsage.UniformBuffer | BufferUsage.Dynamic);
		DeviceBuffer buffer = factory.CreateBuffer(desc);

		return buffer;
	}
}

public readonly ref struct ConstantBufferKey(string name, uint size) : IResourceKey
{
	public uint Size => size;
	public ReadOnlySpan<char> Name => name;

	/// <inheritdoc/>
	public int Hash => unchecked(string.GetHashCode(name));

	public bool Equals(ConstantBufferKey other) => name.SequenceEqual(other.Name);

	/// <inheritdoc/>
	public override int GetHashCode() => Hash;
}
