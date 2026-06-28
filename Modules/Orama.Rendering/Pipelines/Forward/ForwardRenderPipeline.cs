
namespace Orama.Rendering.Pipelines.Forward;

public class ForwardRenderPipeline : RenderPipeline
{
    /// <inheritdoc/>
    public override RenderPass[] Passes { get; } = [new OpaquePass()];
}
