// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System;

using Veldrith;


namespace Orama.Rendering.Resources.Caches;

public sealed class SamplerCache : ResourceCache<SamplerCache, SamplerKey, Veldrith.Sampler>
{
	/// <inheritdoc/>
	protected override Veldrith.Sampler Create(SamplerKey key)
	{
		SamplerDescription desc = new SamplerDescription()
		{
			Filter = GetVeldrithFilter(key.Sampler.Filter),
			AddressModeU = GetVeldrithAddressMode(key.Sampler.WrapU),
			AddressModeV = GetVeldrithAddressMode(key.Sampler.WrapV)
		};

		return Renderer.Veldrith.GraphicsDevice.ResourceFactory.CreateSampler(desc);
	}

	private static Veldrith.SamplerFilter GetVeldrithFilter(SamplerFilter filter)
	{
		return filter switch
		{
			SamplerFilter.Nearest => Veldrith.SamplerFilter.MinPointMagPointMipPoint,
			SamplerFilter.Linear => Veldrith.SamplerFilter.MinLinearMagLinearMipPoint,
			_ => throw new NotSupportedException()
		};
	}

	private static Veldrith.SamplerAddressMode GetVeldrithAddressMode(TextureWrapMode mode)
	{
		return mode switch
		{
			TextureWrapMode.Clamp => Veldrith.SamplerAddressMode.Clamp,
			TextureWrapMode.Repeat => Veldrith.SamplerAddressMode.Wrap,
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
