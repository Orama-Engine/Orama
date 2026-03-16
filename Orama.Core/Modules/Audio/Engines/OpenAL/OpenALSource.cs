using Orama.Core.Modules.Audio.Resources;
using Silk.NET.OpenAL;

namespace Orama.Core.Modules.Audio.Engines.OpenAL;

public class OpenALSource : IAudioSource
{
    private readonly AL al;
    private uint source;

    public OpenALSource(AL al)
    {
        this.al = al;
        source = al.GenSource();
    }

    /// <inheritdoc/>
    public float Volume
    {
        get
        {
            float value = 0f;
            al.GetSourceProperty(source, SourceFloat.Gain, out value);
            return value;
        }
        set => al.SetSourceProperty(source, SourceFloat.Gain, value);
    }

    /// <inheritdoc/>
    public float Pitch
    {
        get
        {
            float value = 0f;
            al.GetSourceProperty(source, SourceFloat.Pitch, out value);
            return value;
        }
        set => al.SetSourceProperty(source, SourceFloat.Pitch, value);
    }

    /// <inheritdoc/>
    public bool Loop
    {
        get
        {
            bool value = false;
            al.GetSourceProperty(source, SourceBoolean.Looping, out value);
            return value;
        }
        set => al.SetSourceProperty(source, SourceBoolean.Looping, value);
    }

    /// <inheritdoc/>
    public bool IsPlaying
    {
        get
        {
            int value = 0;
            al.GetSourceProperty(source, GetSourceInteger.SourceState, out value);
            return value == (int)SourceState.Playing;
        }
    }

    /// <inheritdoc/>
    public void SetClip(AudioClip clip)
    {
        var buffer = al.GenBuffer();
        var format = clip.Channels == 1
            ? clip.BitsPerSample == 8 ? BufferFormat.Mono8 : BufferFormat.Mono16
            : clip.BitsPerSample == 8 ? BufferFormat.Stereo8 : BufferFormat.Stereo16;

        al.BufferData(buffer, format, clip.Data, clip.SampleRate);
        al.SetSourceProperty(source, SourceInteger.Buffer, buffer);
    }

    /// <inheritdoc/>
    public void Play() => al.SourcePlay(source);

    /// <inheritdoc/>
    public void Stop() => al.SourceStop(source);

    /// <inheritdoc/>
    public void Destroy() => al.DeleteSource(source);
}
