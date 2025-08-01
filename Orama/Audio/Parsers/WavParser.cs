using Orama.Audio.Formats;

namespace Orama.Audio;

public static class WavParser
{
	public static WavFile FromFile(string path)
	{
		using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
		using var reader = new BinaryReader(fs);
		
		var riff = new string(reader.ReadChars(4));
		if (riff != "RIFF")
			throw new FormatException("Unable to find 'RIFF' header.");
		
		reader.ReadInt32(); // Skip inbetween bytes
		
		var wave = new string(reader.ReadChars(4));
		if (wave != "WAVE")
			throw new FormatException("Unable to find 'WAVE' header.");
		
		int audioFormat = 0;
		int channels = 0;
		int sampleRate = 0;
		int bitsPerSample = 0;
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
					channels = reader.ReadInt16();
					sampleRate = reader.ReadInt32();
					reader.ReadInt32(); // Byte rate, skipped
					reader.ReadInt16(); // Block align, skipped
					bitsPerSample = reader.ReadInt16();
					
					// Skip extra bytes in the fmt chunk
					if (chunkSize > 16)
						reader.ReadBytes(chunkSize - 16);
					break;
				
				case "data":
					pcmData = reader.ReadBytes(chunkSize);
					break;
				
				default:
					// Skip unhandled chunk
					reader.ReadBytes(chunkSize);
					break;
			}
			
			if (chunkSize % 2 == 1)
				reader.ReadByte(); // Skip padding byte
		}
		
		if (pcmData == null)
			throw new InvalidDataException("No PCM data found in WAV file.");
		
		return new WavFile(channels, sampleRate, bitsPerSample, pcmData);
	}
}