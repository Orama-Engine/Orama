
namespace Orama.Core.Modules.Rendering.Pipelines.Forward;

public class ForwardRenderPipeline : RenderPipeline
{
    /// <inheritdoc/>
    public override RenderPass[] Passes { get; } = [new OpaquePass()];
}
