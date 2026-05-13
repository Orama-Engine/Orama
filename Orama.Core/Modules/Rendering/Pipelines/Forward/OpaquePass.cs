
using Orama.Core.Common.Components;
using Orama.Core.Common.Utility;
using Orama.Rendering;
using Orama.Rendering.Device;

namespace Orama.Core.Modules.Rendering.Pipelines.Forward;

/// <summary> Pass responsible for rendering all solid objects. </summary>
public class OpaquePass : RenderPass
{
    public override void Render()
    {
        CommandBuffer buffer = Renderer.CreateCommandBuffer();

        buffer.CommandList.Begin();
        buffer.CommandList.ClearColorTarget(0, new Veldrid.RgbaFloat(0, 0, 0, 1));
        buffer.CommandList.End();

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
