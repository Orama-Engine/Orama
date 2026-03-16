
using Orama.Core.Modules.Audio.Engines.OpenAL;

namespace Orama.Core.Modules.Audio;

/// <summary>
/// Module responsible for handling audio.
/// </summary>
public class AudioModule : BaseModule, IAudioContext
{
    private readonly IAudioContext context;

    /// <inheritdoc/>
    public IAudioSource CreateSource() => context.CreateSource();

    /// <inheritdoc/>
    public void DestroySource(IAudioSource source) => context.DestroySource(source);

    /// <summary> Creates a new audio module with the given audio context. </summary>
    /// <param name="context"> The audio context to use. </param>
    public AudioModule(IAudioContext context) => this.context = context;

    /// <summary> Creates a new audio module with the default audio context. </summary>
    public AudioModule() => this.context = new OpenALContext();
}