using Orama.Core.Common.Utility;
using Orama.Core.Modules.Audio.Resources;

namespace Orama.Core.Common.Resources.DefaultProvider;

[ResourceLoader]
internal class AudioClipLoader : ResourceLoader<AudioClip>
{
    public override AudioClip? LoadResource(byte[] data)
    {
        using var reader = new BinaryReader(new MemoryStream(data));

        // RIFF header
        reader.ReadBytes(4);
        reader.ReadInt32();
        reader.ReadBytes(4);

        // fmt chunk
        reader.ReadBytes(4);
        reader.ReadInt32();
        reader.ReadInt16();
        int channels = reader.ReadInt16();
        int sampleRate = reader.ReadInt32();
        reader.ReadInt32();
        reader.ReadInt16();
        int bitsPerSample = reader.ReadInt16();

        try
        {
            while (true)
            {
                string chunkId = new string(reader.ReadChars(4));
                int chunkSize = reader.ReadInt32();
                if (chunkId == "data")
                {
                    byte[] pcmData = reader.ReadBytes(chunkSize);
                    return new AudioClip(pcmData, sampleRate, channels, bitsPerSample);
                }
                reader.ReadBytes(chunkSize);
            }
        }
        catch (EndOfStreamException)
        {
            EngineOutput.Warning("WAV file is malformed or missing data chunk.");
            return null;
        }
    }
}
