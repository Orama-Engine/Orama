using Orama.Core.Modules.Audio.Resources;
using Orama.Math;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.Creative;

namespace Orama.Core.Modules.Audio.Engines.OpenAL;

public class OpenALSource : IAudioSource
{
    private readonly AL al;
    private readonly EffectExtension efx;
    private uint source;
    private uint filter;

    public OpenALSource(AL al, EffectExtension efx)
    {
        this.al = al;
        this.efx = efx;
        source = al.GenSource();
        filter = efx.GenFilter();
        efx.SetFilterProperty(filter, FilterInteger.FilterType, (int)FilterType.Lowpass);
    }

    /// <inheritdoc/>
    public Vector3 Position { get; set; }
    /// <inheritdoc/>
    public float Obstruction { get; set; } = 0f;
    /// <inheritdoc/>
    public float Occlusion { get; set; } = 0f;
    /// <inheritdoc/>
    public bool Obstructed { get; set; } = false;

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
    public void Update()
    {
        al.SetSourceProperty(source, SourceVector3.Position, Position.X, Position.Y, Position.Z);
        float gainHF = 1.0f - MathF.Min(1.0f, MathF.Max(0f, Obstruction));

        efx.SetFilterProperty(filter, FilterFloat.LowpassGain, 1.0f);
        efx.SetFilterProperty(filter, FilterFloat.LowpassGainHF, gainHF);
        al.SetSourceProperty(source, (SourceInteger)0x20005, filter);
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
    public void Destroy()
    {
        al.SetSourceProperty(source, (SourceInteger)0x20005, 0);
        efx.DeleteFilter(filter);
        al.DeleteSource(source);
    }
}
