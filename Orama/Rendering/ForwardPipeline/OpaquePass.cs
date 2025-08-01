namespace Orama.Rendering.ForwardPipeline;

/// <summary>
/// Handles rendering opaque (non-transparent) objects.
/// </summary>
public class OpaquePass : RenderPass
{
	public OpaquePass() : base("Opaque") { }

	public override void Execute(RenderContext context)
	{
		Renderer.Render(context.ViewMatrix, context.ProjectionMatrix);
	}
}
