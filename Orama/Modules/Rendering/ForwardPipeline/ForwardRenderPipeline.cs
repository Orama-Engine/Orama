namespace Orama.Modules.Rendering.ForwardPipeline;

/// <summary>
/// The default forward rendering pipeline.
/// </summary>
public class ForwardRenderPipeline : RenderPipeline
{
	public override void Initialize()
	{
		AddPass(new ClearPass());
		AddPass(new OpaquePass());
	}
}
