
using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering.Pipelines;

/// <summary>
/// Render instructions for a step in the rendering pipeline.
/// </summary>
public abstract class RenderPass
{
    public abstract void Render();

    /// <summary> Queues a renderable object to be rendered during the next frame. </summary>
    public void QueueObject(IClientRenderable renderable)
    {
        // TODO: Instantiating a new RenderItem multiple times every frame is very expensive, don't do this

        //
    }
}
