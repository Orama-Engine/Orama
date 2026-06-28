using Orama.Core.Common.Components;
using Orama.Core.Modules.Audio.Resources;
using Orama.Modules;

namespace Orama.Core.Modules.Audio.Components;

public class AudioListener : Component
{
    private IAudioListener? listener;

    public override void Start()
    {
        var audio = ModuleManager.GetModule<AudioModule>();
        if (audio != null)
        {
            listener = audio.CreateListener();
            audio.SetListener(listener);
        }
    }

    public override void Update()
    {
        if (listener == null) return;

        var transform = Entity.Transform;
        listener.Position = transform.Position;
        listener.Forward = -transform.Forward;
        listener.Up = transform.Up;
        listener.Update();
    }

    public override void Destroy()
    {
        base.Destroy();
        var audio = ModuleManager.GetModule<AudioModule>();
        if (audio != null)
        {
            if (listener != null) audio.DestroyListener(listener);
            audio.SetListener(null);
        }
    }
}