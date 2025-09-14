using Orama.Modules;
using Orama.Modules.Audio;
using Orama.Resources.ResourceLibrary;
using Silk.NET.OpenAL;

namespace Orama.Resources;

public class Audio : IResource<Audio>
{
	public int Channels { get; private set; }
	public int SampleRate { get; private set; }
	public int BitsPerSample { get; private set; }
	public byte[] PcmData { get; private set; }
	
	// Constructor
	public Audio() { }

	public Audio Deserialize(Stream stream)
	{
		using var reader = new BinaryReader(stream);

		var riff = new string(reader.ReadChars(4));
		if (riff != "RIFF")
			throw new FormatException("Unable to find 'RIFF' header.");

		reader.ReadInt32(); // skip

		var wave = new string(reader.ReadChars(4));
		if (wave != "WAVE")
			throw new FormatException("Unable to find 'WAVE' header.");

		int audioFormat = 0;
		byte[] pcmData = null;

		while (reader.BaseStream.Position < reader.BaseStream.Length)
		{
			string chunkId = new string(reader.ReadChars(4));
			int chunkSize = reader.ReadInt32();

			switch (chunkId)
			{
				case "fmt ":
					audioFormat = reader.ReadInt16();
					if (audioFormat != 1)
						throw new NotSupportedException("Unsupported audio format.");

					Channels = reader.ReadInt16();
					SampleRate = reader.ReadInt32();
					reader.ReadInt32(); // skip byte rate
					reader.ReadInt16(); // skip block align
					BitsPerSample = reader.ReadInt16();

					if (chunkSize > 16)
						reader.ReadBytes(chunkSize - 16);
					break;

				case "data":
					pcmData = reader.ReadBytes(chunkSize);
					break;

				default:
					reader.ReadBytes(chunkSize);
					break;
			}

			if (chunkSize % 2 == 1)
				reader.ReadByte();
		}

		if (pcmData == null)
			throw new InvalidDataException("No PCM data found in WAV file.");

		PcmData = pcmData;

		return this;
	}
	
	public void Serialize(Stream stream) { }
	
	/// <summary>
	/// Maps channels & bits to the OpenAL BufferFormat enum.
	/// </summary>
	/// <returns>BufferFormat</returns>
	/// <exception cref="NotSupportedException">Unsupported format.</exception>
	public BufferFormat GetFormat()
	{
		return (Channels, BitsPerSample) switch
		{
			(1, 8) => BufferFormat.Mono8,
			(1, 16) => BufferFormat.Mono16,
			(2, 8) => BufferFormat.Stereo8,
			(2, 16) => BufferFormat.Stereo16,
			_ => throw new NotSupportedException($"Unsupported WAV format: {Channels} channels, {BitsPerSample} bits per sample")
		};
	}
	
	/// <summary>
	/// Plays the sound resource.
	/// </summary>
	public void Play()
	{
		var format = GetFormat();

		var buffer = ModuleManager.GetModule<AudioModule>().CreateBuffer(
			PcmData,
			format,
			PcmData.Length,
			SampleRate
		);

	var source = ModuleManager.GetModule<AudioModule>().GenerateSource();
	ModuleManager.GetModule<AudioModule>().AttachBufferToSource(source, buffer);
	ModuleManager.GetModule<AudioModule>().PlaySource(source);
	}
}