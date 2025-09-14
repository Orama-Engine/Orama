namespace Orama.Modules.Rendering;

/// <summary>
/// Manages render pipelines.
/// </summary>
public static class RenderPipelineManager
{
	/// <summary> The current render pipeline. </summary>
	public static RenderPipeline Current { get; set; } = new ForwardPipeline.ForwardRenderPipeline();
	
	/// <summary> Called once per frame to submit rendering tasks. </summary>
	public static void RenderFrame(RenderContext context)
	{
		ModuleManager.GetModule<RendererModule>().BeginFrame();
		Current.RenderFrame(context);
		ModuleManager.GetModule<RendererModule>().EndFrame();
	}
}
