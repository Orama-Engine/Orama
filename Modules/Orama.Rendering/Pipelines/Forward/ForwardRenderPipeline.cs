
namespace Orama.Rendering.Pipelines.Forward;

public class ForwardRenderPipeline : RenderPipeline
{
    /// <inheritdoc/>
    public override RenderPass[] Passes { get; }

    public ForwardRenderPipeline()
    {
        Passes = [new OpaquePass() { Pipeline = this }]; // Hack
    }
}
