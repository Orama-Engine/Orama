// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Resources.Caches;

public sealed class TextureCache : ResourceCache<TextureCache, TextureKey, Veldrith.Texture>
{
	/// <inheritdoc/>
	protected override Veldrith.Texture Create(TextureKey key)
	{
		var desc = new Veldrith.TextureDescription(key.Width, key.Height, 1, 1, 1, GetVeldrithFormat(key.Format), Veldrith.TextureUsage.Sampled, Veldrith.TextureType.Texture2D);

		Veldrith.Texture texture = Renderer.Device.ResourceFactory.CreateTexture(desc);

		Renderer.Device.UpdateTexture(texture, key.Data, 0, 0, 0, key.Width, key.Height, 1, 0, 0);

		return texture;
	}

	private static Veldrith.PixelFormat GetVeldrithFormat(TextureFormat format) => format switch
	{
		TextureFormat.RGB8 => Veldrith.PixelFormat.R8G8B8A8UNorm,
		TextureFormat.RGBA8 => Veldrith.PixelFormat.R8G8B8A8UNorm,
		_ => throw new NotSupportedException($"Unsupported texture format: {format}")
	};
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
