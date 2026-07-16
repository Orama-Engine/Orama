// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System;

namespace Orama.Rendering.Resources.Caches;

public sealed class TextureCache : ResourceCache<TextureCache, TextureKey, NeoVeldrid.Texture>
{
	/// <inheritdoc/>
	protected override NeoVeldrid.Texture Create(TextureKey key)
	{
		NeoVeldrid.TextureDescription desc = new NeoVeldrid.TextureDescription(key.Width, key.Height, 1, 1, 1, GetVeldridFormat(key.Format), NeoVeldrid.TextureUsage.Sampled, NeoVeldrid.TextureType.Texture2D);

		NeoVeldrid.Texture texture = Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateTexture(desc);

		Renderer.Veldrid.GraphicsDevice.UpdateTexture(texture, key.Data, 0, 0, 0, key.Width, key.Height, 1, 0, 0);

		return texture;
	}

	private static NeoVeldrid.PixelFormat GetVeldridFormat(TextureFormat format)
	{
		return format switch
		{
			TextureFormat.RGB8 => NeoVeldrid.PixelFormat.R8_G8_B8_A8_UNorm,
			TextureFormat.RGBA8 => NeoVeldrid.PixelFormat.R8_G8_B8_A8_UNorm,
			_ => throw new NotSupportedException($"Unsupported texture format: {format}")
		};
	}
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
		if (Width != other.Width) return false;
		if (Height != other.Height) return false;
		if (Format != other.Format) return false;
		if (!Data.SequenceEqual(other.Data)) return false;
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
			foreach (var b in Data) hash = hash * 31 + b;
			return hash;
		}
	}
}
