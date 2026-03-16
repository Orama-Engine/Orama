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

        // data chunk
        reader.ReadBytes(4);
        int dataSize = reader.ReadInt32();
        byte[] pcmData = reader.ReadBytes(dataSize);

        return new AudioClip(pcmData, sampleRate, channels, bitsPerSample);
    }
}
