namespace Orama.Modules.Rendering;

/// <summary>
/// Manages render pipelines.
/// </summary>
public static class RenderPipelineManager
{
	/// <summary> The current render pipeline. </summary>
	public static RenderPipeline Current { get; set; } = new ForwardPipeline.ForwardRenderPipeline();
	
	private static RendererModule rendererModule => ModuleManager.GetModule<RendererModule>()
	                                         ?? throw new InvalidOperationException("RendererModule must exist and be initialized.");

	/// <summary> Called once per frame to submit rendering tasks. </summary>
	public static void RenderFrame(RenderContext context)
	{
		rendererModule.BeginFrame();
		Current.RenderFrame(context);
		rendererModule.EndFrame();
	}
}
