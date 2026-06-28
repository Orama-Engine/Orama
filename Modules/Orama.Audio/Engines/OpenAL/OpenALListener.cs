using Orama.Math;
using Silk.NET.OpenAL;


namespace Orama.Audio.Engines.OpenAL;

public class OpenALListener : IAudioListener
{
    private readonly AL al;

    public OpenALListener(AL al)
    {
        this.al = al;
    }

    /// <inheritdoc/>
    public Vector3 Position { get; set; } = Vector3.Zero;

    /// <inheritdoc/>
    public Vector3 Forward { get; set; } = Vector3.UnitZ;

    /// <inheritdoc/>
    public Vector3 Up { get; set; } = Vector3.UnitY;

    /// <inheritdoc/>
    public float Gain
    {
        get
        {
            float value = 0f;
            al.GetListenerProperty(ListenerFloat.Gain, out value);
            return value;
        }
        set => al.SetListenerProperty(ListenerFloat.Gain, value);
    }

    /// <inheritdoc/>
    public unsafe void Update()
    {
        al.SetListenerProperty(ListenerVector3.Position, Position.X, Position.Y, Position.Z);

        float* orientation = stackalloc float[]
        {
            Forward.X, Forward.Y, Forward.Z,
            Up.X, Up.Y, Up.Z
        };

        al.SetListenerProperty(ListenerFloatArray.Orientation, orientation);
    }
}
