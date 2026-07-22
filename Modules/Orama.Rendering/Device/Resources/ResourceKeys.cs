// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)


using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;
using System.Runtime.CompilerServices;

namespace Orama.Rendering.Device.Resources;

public readonly ref struct ConstantBufferKey(string name, uint size) : IResourceKey
{
	public uint Size => size;
	public ReadOnlySpan<char> Name => name;

	/// <inheritdoc/>
	public override int GetHashCode() => unchecked(string.GetHashCode(name));
}


public readonly ref struct PipelineKey(string passName, ShaderKey vertShader, ShaderKey fragShader, IFramebuffer output, ReadOnlySpan<ShaderResourceGroup> resourceGroups) : IResourceKey
{
	public readonly string PassName = passName;
	public readonly ShaderKey VertShader = vertShader;
	public readonly ShaderKey FragShader = fragShader;
	public readonly IFramebuffer Output = output;
	public readonly ReadOnlySpan<ShaderResourceGroup> ResourceGroups = resourceGroups;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		hash.Add(PassName);
		hash.Add(VertShader.Hash);
		hash.Add(FragShader.Hash);
		hash.Add(Output);
		foreach (ref readonly var group in ResourceGroups)
		{
			hash.Add(group.Set);
			foreach (var element in group.LayoutElements)
			{
				hash.Add(element.Name);
				hash.Add(element.Kind);
				hash.Add(element.Stages);
			}
		}
		return hash.ToHashCode();
	}
}


public readonly ref struct SamplerKey(Sampler sampler) : IResourceKey
{
	public readonly Sampler Sampler = sampler;

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

public readonly ref struct TextureKey(uint width, uint height, TextureFormat format, ReadOnlySpan<byte> data) : IResourceKey
{
	public readonly uint Width = width;
	public readonly uint Height = height;
	public readonly TextureFormat Format = format;
	public readonly ReadOnlySpan<byte> Data = data;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + (int)Width;
			hash = hash * 31 + (int)Height;
			hash = hash * 31 + (int)Format;
			foreach (byte b in Data)
				hash = hash * 31 + b;
			return hash;
		}
	}
}

public readonly ref struct TextureViewKey(ITexture texture) : IResourceKey
{
	public readonly ITexture Texture = texture;

	/// <inheritdoc/>
	public override int GetHashCode() => RuntimeHelpers.GetHashCode(Texture);
}
