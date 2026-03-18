
namespace Orama.Core.Modules.Audio;

/// <summary>
/// Represents an audio context responsible for managing audio sources.
/// </summary>
public interface IAudioContext
{
    /// <summary> Creates a new audio source and adds it to the context. </summary>
    /// <returns> The created audio source. </returns>
    IAudioSource CreateSource();

    /// <summary> Destroys an audio source and removes it from the context. </summary>
    /// <param name="source"> The audio source to remove. </param>
    void DestroySource(IAudioSource source);
}