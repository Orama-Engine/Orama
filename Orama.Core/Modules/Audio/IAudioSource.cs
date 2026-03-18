
using Orama.Core.Modules.Audio.Resources;

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

    /// <summary> Sets the audio clip to be used for playback. </summary>
    /// <param name="clip"> The audio clip to assign. </param>
    void SetClip(AudioClip clip);

    /// <summary> Plays the audio source. </summary>
    void Play();

    /// <summary> Stops the audio source. </summary>
    void Stop();

    /// <summary> Destroys the audio source. </summary>
    void Destroy();
}
