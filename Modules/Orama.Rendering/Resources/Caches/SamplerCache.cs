// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System;

using NeoVeldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class SamplerCache : ResourceCache<SamplerCache, SamplerKey, NeoVeldrid.Sampler>
{
	/// <inheritdoc/>
	protected override NeoVeldrid.Sampler Create(SamplerKey key)
	{
		SamplerDescription desc = new SamplerDescription()
		{
			Filter = GetVeldridFilter(key.Sampler.Filter),
			AddressModeU = GetVeldridAddressMode(key.Sampler.WrapU),
			AddressModeV = GetVeldridAddressMode(key.Sampler.WrapV)
		};

		return Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateSampler(desc);
	}

	private static NeoVeldrid.SamplerFilter GetVeldridFilter(SamplerFilter filter)
	{
		return filter switch
		{
			SamplerFilter.Nearest => NeoVeldrid.SamplerFilter.MinPoint_MagPoint_MipPoint,
			SamplerFilter.Linear => NeoVeldrid.SamplerFilter.MinLinear_MagLinear_MipPoint,
			_ => throw new NotSupportedException()
		};
	}

	private static NeoVeldrid.SamplerAddressMode GetVeldridAddressMode(TextureWrapMode mode)
	{
		return mode switch
		{
			TextureWrapMode.Clamp => NeoVeldrid.SamplerAddressMode.Clamp,
			TextureWrapMode.Repeat => NeoVeldrid.SamplerAddressMode.Wrap,
			_ => throw new NotSupportedException()
		};
	}
}

public readonly ref struct SamplerKey(Sampler sampler) : IResourceKey
{
	public readonly Sampler Sampler = sampler;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(SamplerKey other)
	{
		if (Sampler.Filter != other.Sampler.Filter) return false;
		if (Sampler.WrapU != other.Sampler.WrapU) return false;
		if (Sampler.WrapV != other.Sampler.WrapV) return false;
		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + (int)Sampler.Filter;
			hash = hash * 31 + (int)Sampler.WrapU;
			hash = hash * 31 + (int)Sampler.WrapV;
			return hash;
		}
	}
}
