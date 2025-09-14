namespace Orama.Modules.Rendering.ForwardPipeline;

/// <summary>
/// Clears the screen.
/// </summary>
public class ClearPass : RenderPass
{
	public ClearPass() : base("Clear") { }
	
	private RendererModule rendererModule => ModuleManager.GetModule<RendererModule>()
	                                   ?? throw new InvalidOperationException("RendererModule must exist and be initialized.");

	public override void Execute(RenderContext context)
	{
		rendererModule.Clear(new System.Numerics.Vector4(0f, 0f, 0f, 1f));
		rendererModule.ClearDepth(1f);
	}
}
