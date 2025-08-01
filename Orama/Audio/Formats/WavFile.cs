using Silk.NET.OpenAL;

namespace Orama.Audio.Formats;

public class WavFile
{
	public int Channels { get; }
	public int SampleRate { get; }
	public int BitsPerSample { get; }
	public byte[] PcmData { get; }
	
	public WavFile(int channels, int sampleRate, int bitsPerSample, byte[] pcmData)
	{
		if (channels <= 0 || sampleRate <= 0 || bitsPerSample <= 0 || pcmData == null || pcmData.Length == 0)
			throw new InvalidDataException("WAV file failed to construct.");
		
		Channels = channels;
		SampleRate = sampleRate;
		BitsPerSample = bitsPerSample;
		PcmData = pcmData;
	}

	/// <summary>
	/// Maps channels & bits to the OpenAL BufferFormat enum.
	/// </summary>
	/// <returns>BufferFormat</returns>
	/// <exception cref="NotSupportedException">Unsupported WAV format.</exception>
	public BufferFormat GetFormat()
	{
		return (Channels, BitsPerSample) switch
		{
			(1, 8) => BufferFormat.Mono8,
			(1, 16) => BufferFormat.Mono16,
			(2, 8) => BufferFormat.Stereo8,
			(2, 16) => BufferFormat.Stereo16,
			_ => throw new NotSupportedException(
				$"Unsupported WAV format: {Channels} channels, {BitsPerSample} bits per sample")
		};
	}
}