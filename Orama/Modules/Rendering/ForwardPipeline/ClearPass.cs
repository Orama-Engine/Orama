namespace Orama.Modules.Rendering.ForwardPipeline;

/// <summary>
/// Clears the screen.
/// </summary>
public class ClearPass : RenderPass
{
	public ClearPass() : base("Clear") { }
	
	public override void Execute(RenderContext context)
	{
		ModuleManager.GetModule<RendererModule>().Clear(new System.Numerics.Vector4(0f, 0f, 0f, 1f));
		ModuleManager.GetModule<RendererModule>().ClearDepth(1f);
	}
}
