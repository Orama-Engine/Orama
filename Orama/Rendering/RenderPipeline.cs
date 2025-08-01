using System;

namespace Orama.Rendering;

public abstract class RenderPipeline : IDisposable
{
	/// <summary>
	/// List of registered render passes in execution order.
	/// </summary>
	protected readonly List<RenderPass> Passes = new();

	/// <summary>
	/// Called once during engine initialization.
	/// </summary>
	public virtual void Initialize() { }

	/// <summary>
	/// Called once per frame to submit rendering tasks.
	/// </summary>
	public void RenderFrame(RenderContext context)
	{
		foreach (var pass in Passes)
			pass.Execute(context);
	}

	/// <summary>
	/// Called when the pipeline or window is destroyed.
	/// </summary>
	public virtual void Dispose()
	{
		foreach (var pass in Passes)
			pass.Dispose();

		Passes.Clear();
	}

	/// <summary>
	/// Adds a render pass to the pipeline.
	/// </summary>
	public void AddPass(RenderPass pass) => Passes.Add(pass);

	/// <summary>
	/// Removes a render pass from the pipeline.
	/// </summary>
	public void RemovePass(RenderPass pass) => Passes.Remove(pass);
}
