using Orama.Core.Modules.Audio.Engines.OpenAL;
using Orama.Core.Modules.Scenes;

namespace Orama.Core.Modules.Audio;

/// <summary>
/// Module responsible for handling audio.
/// </summary>
public class AudioModule : BaseModule, IAudioContext
{
    private readonly IAudioContext context;

    private readonly AudioPhysics audioPhysics = new();

    /// <inheritdoc/>
    public IAudioSource CreateSource() => context.CreateSource();

    /// <inheritdoc/>
    public void DestroySource(IAudioSource source) => context.DestroySource(source);

    /// <inheritdoc/>
    public IAudioListener CreateListener() => context.CreateListener();

    /// <inheritdoc/>
    public void DestroyListener(IAudioListener listener) => context.DestroyListener(listener);

    /// <inheritdoc/>
    public void SetListener(IAudioListener? listener) => context.SetListener(listener);

    /// <inheritdoc/>
    public void SetDistanceModel(AudioDistanceModel model) => context.SetDistanceModel(model); 

    /// <summary> Creates a new audio module with the given audio context. </summary>
    /// <param name="context"> The audio context to use. </param>
    public AudioModule(IAudioContext context) => this.context = context;

    /// <summary> Creates a new audio module with the default audio context. </summary>
    public AudioModule() => this.context = new OpenALContext();

    public override void Update()
    {
        var activeScene = ModuleManager.GetModule<SceneModule>()?.CurrentScene;
        if (activeScene != null) audioPhysics.Update(activeScene);
    }
}

/// <summary> Defines how audio source volume attenuates with distance from the listener. </summary>
public enum AudioDistanceModel
{
    None,
    InverseDistance,
    InverseDistanceClamped,
    LinearDistance,
    LinearDistanceClamped,
    ExponentDistance,
    ExponentDistanceClamped
}