
namespace Orama.Core.Modules.Audio;

public class AudioModule : BaseModule
{
    /// <summary> The audio context. </summary>
    public IAudioContext? Context { get; set; }

    /// <summary> Creates a new audio module with the given audio context. </summary>
    /// <param name="context"> The audio context to use. </param>
    public AudioModule(IAudioContext context) => Context = context;
}