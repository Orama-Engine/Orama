// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Audio.Resources;

public class AudioClip
{
	public byte[] Data { get; }
	public int SampleRate { get; }
	public int Channels { get; }
	public int BitsPerSample { get; }

	public AudioClip(byte[] data, int sampleRate, int channels, int bitsPerSample)
	{
		Data = data;
		SampleRate = sampleRate;
		Channels = channels;
		BitsPerSample = bitsPerSample;
	}
}
