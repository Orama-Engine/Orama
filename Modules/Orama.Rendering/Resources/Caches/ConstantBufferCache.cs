// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)


using Orama.Rendering.Device.Resources;

namespace Orama.Rendering.Resources.Caches;

public sealed class ConstantBufferCache : ResourceCache<ConstantBufferCache, ConstantBufferKey, IBuffer>
{
	/// <inheritdoc/>
	protected override IBuffer Create(ConstantBufferKey key) => Renderer.Device.ResourceFactory.CreateBuffer(new BufferKey(key.Size, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
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
