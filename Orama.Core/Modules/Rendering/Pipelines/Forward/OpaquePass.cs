
using Orama.Core.Common.Components;
using Orama.Core.Common.Utility;
using Orama.Rendering;
using Orama.Rendering.Backends;

namespace Orama.Core.Modules.Rendering.Pipelines.Forward;

/// <summary> Pass responsible for rendering all solid objects. </summary>
public class OpaquePass : RenderPass
{
    public override void Render()
    {
        Renderer.CommandBuffer.EnableFeature(RenderFeature.DepthTest);
        Renderer.CommandBuffer.SetDepthMask(true);

        Renderer.CommandBuffer.Clear(0f, 0f, 0f, 1f);

        Renderer.CommandBuffer.DisableFeature(RenderFeature.Blending);
        Renderer.CommandBuffer.EnableFeature(RenderFeature.CullFaces);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Pass == "Opaque")
                QueueObject(renderable);

        // TODO: Better camera target handling
        if (Camera.Main?.Target != null)
        {
            Renderer.RenderToTarget(Camera.Main.Target.GraphicsTexture, Camera.Main != null ? (System.Numerics.Matrix4x4)Camera.Main.ViewMatrix : System.Numerics.Matrix4x4.Identity, Camera.Main != null ? (System.Numerics.Matrix4x4)Camera.Main.ProjectionMatrix : System.Numerics.Matrix4x4.Identity);
        } 
        else
        {
            Renderer.Render(Camera.Main != null ? (System.Numerics.Matrix4x4)Camera.Main.ViewMatrix : System.Numerics.Matrix4x4.Identity, Camera.Main != null ? (System.Numerics.Matrix4x4)Camera.Main.ProjectionMatrix : System.Numerics.Matrix4x4.Identity);
        }
    }
}
