using System;
using System.Collections.Generic;
using System.Text;

namespace Orama.Core.Modules.Audio.Resources;

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