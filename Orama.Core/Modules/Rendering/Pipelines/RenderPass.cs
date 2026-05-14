
using Orama.Rendering;
using Orama.Rendering.Device;
using Orama.Rendering.Resources;
using Veldrid;
using Vulkan;

namespace Orama.Core.Modules.Rendering.Pipelines;

/// <summary>
/// Render instructions for a step in the rendering pipeline.
/// </summary>
public abstract class RenderPass
{
    public abstract void Render();
}
