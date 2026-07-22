// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device.Resources;


namespace Orama.Rendering.Resources.Caches;

public sealed class SamplerCache : ResourceCache<SamplerCache, SamplerKey, ISampler>
{
	/// <inheritdoc/>
	protected override ISampler Create(SamplerKey key) => Renderer.Device.ResourceFactory.CreateSampler(key);
}

public readonly ref struct SamplerKey(Sampler sampler) : IResourceKey
{
	public readonly Sampler Sampler = sampler;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(SamplerKey other)
	{
		if (Sampler.Filter != other.Sampler.Filter)
			return false;
		if (Sampler.WrapU != other.Sampler.WrapU)
			return false;
		if (Sampler.WrapV != other.Sampler.WrapV)
			return false;
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
