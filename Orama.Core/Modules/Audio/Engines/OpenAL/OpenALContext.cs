using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.Creative;

namespace Orama.Core.Modules.Audio.Engines.OpenAL;

public class OpenALContext : IAudioContext
{
    private readonly AL al;
    private readonly ALContext alc;
    private readonly unsafe Context* context;
    private readonly EffectExtension efx;

    private IAudioListener? activeListener;

    public unsafe OpenALContext()
    {
        alc = ALContext.GetApi();
        al = AL.GetApi();

        var device = alc.OpenDevice(null);
        context = alc.CreateContext(device, null);
        alc.MakeContextCurrent(context);

        efx = new EffectExtension(al.Context);
    }

    /// <inheritdoc/>
    public IAudioSource CreateSource() => new OpenALSource(al, efx);

    /// <inheritdoc/>
    public void DestroySource(IAudioSource source)
    {
        if (source is OpenALSource s)
            s.Destroy();
    }

    /// <inheritdoc/>
    public IAudioListener CreateListener() => new OpenALListener(al);

    /// <inheritdoc/>
    public void DestroyListener(IAudioListener listener) { }

    /// <inheritdoc/>
    public void SetListener(IAudioListener? listener) => activeListener = listener;
}
