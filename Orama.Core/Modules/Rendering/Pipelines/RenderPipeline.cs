
namespace Orama.Core.Modules.Rendering.Pipelines;

/// <summary>
/// A Collection of render passes.
/// </summary>
public abstract class RenderPipeline
{
    /// <summary> The render passes used by the pipeline. </summary>
    public abstract RenderPass[] Passes { get; }

    /// <summary> Renders the pipeline. </summary>
    public void Render() => Array.ForEach(Passes, p => p.Render());
}
