
using Orama.Rendering.Veldrid;
using Veldrid;

namespace Orama.Rendering.Device;

public class CommandBuffer : IDisposable
{
    public CommandList CommandList { get; }

    /// <summary> Initializes a new instance of the <see cref="CommandBuffer"/> class. </summary>
    public CommandBuffer(VeldridDevice device)
    {
        CommandList = device.GraphicsDevice.ResourceFactory.CreateCommandList();
    }

    /// <inheritdoc/>
    public void Dispose() => CommandList.Dispose();
}
