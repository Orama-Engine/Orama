
namespace Orama.Core.Modules.Audio;

/// <summary>
/// Represents an audio source within the audio context.
/// </summary>
public interface IAudioSource
{
    /// <summary> Volume of the audio source. </summary>
    float Volume { get; set; }

    /// <summary> Pitch of the audio source. </summary>
    float Pitch { get; set; }

    /// <summary> Whether the audio source loops. </summary>
    bool Loop { get; set; }

    /// <summary> Whether the audio source is currently playing. </summary>
    bool IsPlaying { get; }


    /// <summary> Plays the audio source. </summary>
    void Play();

    /// <summary> Stops the audio source. </summary>
    void Stop();
}
