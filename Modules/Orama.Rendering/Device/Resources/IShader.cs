// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Resources.Caches;

namespace Orama.Rendering.Device.Resources;

public interface IShader : IGPUResource
{
	public enum ShaderStage : byte
	{
		Vertex,
		Fragment
	}
}

public readonly ref struct ShaderKey(ReadOnlySpan<byte> bytecode, IShader.ShaderStage stage) : IResourceKey
{
	public readonly ReadOnlySpan<byte> Bytecode = bytecode;
	public readonly IShader.ShaderStage Stage = stage;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;

			foreach (byte b in Bytecode)
				hash = hash * 31 + b;

			return hash;
		}
	}
}
