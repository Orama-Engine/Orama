
namespace Orama.Core.Modules.Rendering.Pipelines.Forward;

public class ForwardRenderPipeline : RenderPipeline
{
    public override RenderPass[] Passes { get;  } = new RenderPass[] { new OpaquePass(), new GUIPass() };
}
