namespace Orama.Rendering.ForwardPipeline;

/// <summary>
/// Clears the screen.
/// </summary>
public class ClearPass : RenderPass
{
	public ClearPass() : base("Clear") { }

	public override void Execute(RenderContext context)
	{
		Renderer.Clear(new System.Numerics.Vector4(0f, 0f, 0f, 1f));
		Renderer.ClearDepth(1f);
	}
}
