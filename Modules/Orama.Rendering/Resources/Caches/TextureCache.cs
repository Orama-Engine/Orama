// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Resources.Caches;

using Orama.Rendering.Device.Resources;

public sealed class TextureCache : ResourceCache<TextureCache, TextureKey, ITexture>
{
	/// <inheritdoc/>
	protected override ITexture Create(TextureKey key) => Renderer.Device.ResourceFactory.CreateTexture(key);
}

public readonly ref struct TextureKey(uint width, uint height, TextureFormat format, ReadOnlySpan<byte> data) : IResourceKey
{
	public readonly uint Width = width;
	public readonly uint Height = height;
	public readonly TextureFormat Format = format;
	public readonly ReadOnlySpan<byte> Data = data;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	public bool Equals(TextureKey other)
	{
		if (Width != other.Width)
			return false;
		if (Height != other.Height)
			return false;
		if (Format != other.Format)
			return false;
		if (!Data.SequenceEqual(other.Data))
			return false;
		return true;
	}

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
