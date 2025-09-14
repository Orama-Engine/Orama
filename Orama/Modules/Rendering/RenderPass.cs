
namespace Orama.Modules.Rendering;

/// <summary>
/// A single stage in the rendering pipeline.
/// </summary>
public abstract class RenderPass : IDisposable
{
	/// <summary>
	/// The name of the render pass.
	/// </summary>
	public string Name { get; }

	protected RenderPass(string name) => Name = name;

	/// <summary>
	/// Called each frame to perform this pass.
	/// </summary>
	public abstract void Execute(RenderContext context);

	/// <summary>
	/// Called to free resources.
	/// </summary>
	public virtual void Dispose() { }
}
