
using Veldrid;

namespace Orama.Rendering.Backends.Veldrid;

internal class VeldridBuffer : ICommandBuffer
{
    private CommandList commandList;

    public VeldridBuffer(VeldridBackend backend)
    {
        commandList = backend.Device.ResourceFactory.CreateCommandList();
    }

    /// <inheritdoc/>
    public void Clear(float r, float g, float b, float a)
    {

    }

    /// <inheritdoc/>
    public void DisableFeature(RenderFeature feature)
    {

    }

    /// <inheritdoc/>
    public void EnableFeature(RenderFeature feature)
    {

    }

    /// <inheritdoc/>
    public void SetDepthMask(bool flag)
    {

    }
}
