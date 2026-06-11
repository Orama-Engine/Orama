using Orama.Core.Common.Components;
using Orama.Core.Modules.Audio.Resources;
using Orama.Math;

namespace Orama.Core.Modules.Audio.Components;

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

    /// <summary> The line-of-sight obstruction level of the audio source. </summary>
    public float Obstruction { get; set; } = 0f;

    /// <summary> The acoustic occlusion level of the audio source. </summary>
    public float Occlusion { get; set; } = 0f;

    /// <summary> Whether the audio source's path to the listener is currently blocked. </summary>
    public bool Obstructed { get; set; } = false;

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

    public override void Update()
    {
        if (source == null) return;
        source.Obstruction = Obstructed ? 0.6f : 0f;
        source.Update();
    }

    /// <inheritdoc/>
    public override void Destroy()
    {
        base.Destroy();
        var audio = ModuleManager.GetModule<AudioModule>();
        if (source != null) audio?.DestroySource(source);
    }
}