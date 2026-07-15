// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)


namespace Orama.Rendering.Resources.Caches;

public sealed class TextureCache : ResourceCache<TextureCache, TextureKey, NeoVeldrid.Texture>
{
	/// <inheritdoc/>
	protected override NeoVeldrid.Texture Create(TextureKey key)
	{
		NeoVeldrid.TextureDescription desc = new NeoVeldrid.TextureDescription(key.Width, key.Height, 1, 1, 1, GetVeldridFormat(key.Format), NeoVeldrid.TextureUsage.Sampled, NeoVeldrid.TextureType.Texture2D);

		NeoVeldrid.Texture texture = Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateTexture(desc);

		ReadOnlySpan<byte> data = key.Data.AsSpan();

		Renderer.Veldrid.GraphicsDevice.UpdateTexture(
			texture,
			data,
			0,
			0,
			0,
			key.Width,
			key.Height,
			1,
			0,
			0
		);


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


public readonly record struct TextureKey(uint Width, uint Height, TextureFormat Format, byte[] Data)
{
	public bool Equals(TextureKey other)
	{
		return Width == other.Width
			&& Height == other.Height
			&& Format == other.Format
			&& Data.SequenceEqual(other.Data);
	}

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Width, Height, Format, Data);
}
