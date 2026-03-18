using Orama.Core.Common.Components;
using Orama.Core.Modules.Audio.Resources;

namespace Orama.Core.Modules.Audio.Components;

/// <summary>
/// An audio source component to attach to an entity.
/// </summary>
public class AudioSource : Component
{
    private IAudioSource? source;

    /// <summary> The audio clip to play. </summary>
    public AudioClip? Clip { get; set; }

    /// <summary> Volume of the audio source. </summary>
    public float Volume { get; set; } = 1f;

    /// <summary> Pitch of the audio source. </summary>
    public float Pitch { get; set; } = 1f;

    /// <summary> Whether the audio source loops. </summary>
    public bool Loop { get; set; } = false;
     
    /// <inheritdoc/>
    public override void Start()
    {
        var audio = ModuleManager.GetModule<AudioModule>();

        source = audio?.CreateSource();
        if (source != null)
        {
            source.Volume = Volume;
            source.Pitch = Pitch;
            source.Loop = Loop;
            if (Clip != null) source.SetClip(Clip);
        }
    }

    /// <summary> Plays the audio source. </summary>
    public void Play() => source?.Play();

    /// <summary> Stops the audio source. </summary>
    public void Stop() => source?.Stop();

    /// <inheritdoc/>
    public override void Destroy()
    {
        base.Destroy();
        var audio = ModuleManager.GetModule<AudioModule>();
        if (source != null) audio?.DestroySource(source);
    }
}