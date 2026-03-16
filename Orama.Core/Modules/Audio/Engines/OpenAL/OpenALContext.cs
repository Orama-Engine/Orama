using Silk.NET.OpenAL;

namespace Orama.Core.Modules.Audio.Engines.OpenAL;

public class OpenALContext : IAudioContext
{
    private readonly AL al;
    private readonly ALContext alc;
    private readonly unsafe Context* context;

    public unsafe OpenALContext()
    {
        alc = ALContext.GetApi();
        al = AL.GetApi();

        var device = alc.OpenDevice(null);
        context = alc.CreateContext(device, null);
        alc.MakeContextCurrent(context);
    }

    /// <inheritdoc/>
    public IAudioSource CreateSource() => new OpenALSource(al);

    /// <inheritdoc/>
    public void DestroySource(IAudioSource source)
    {
        if (source is OpenALSource s)
            s.Destroy();
    }
}
