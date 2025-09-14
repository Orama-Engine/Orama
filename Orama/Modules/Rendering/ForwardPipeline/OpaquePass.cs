namespace Orama.Modules.Rendering.ForwardPipeline;

/// <summary>
/// Handles rendering opaque (non-transparent) objects.
/// </summary>
public class OpaquePass : RenderPass
{
	public OpaquePass() : base("Opaque") { }
	
	private RendererModule rendererModule => ModuleManager.GetModule<RendererModule>()
	                                         ?? throw new InvalidOperationException("RendererModule must exist and be initialized.");

	public override void Execute(RenderContext context)
	{
		rendererModule.Render(context.ViewMatrix, context.ProjectionMatrix);
	}
}
