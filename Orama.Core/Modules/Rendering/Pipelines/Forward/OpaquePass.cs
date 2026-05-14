using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Device;
using Veldrid;

namespace Orama.Core.Modules.Rendering.Pipelines.Forward;

/// <summary> Pass responsible for rendering all solid objects. </summary>
public class OpaquePass : RenderPass
{
    public override void Render(ref RenderFrame frame)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;
        var buffer = Renderer.AllocateCommandBuffer();

        buffer.Begin();

        buffer.CommandList.SetFramebuffer(gd.SwapchainFramebuffer);

        buffer.ClearColor(Color.Black);

        Matrix4x4 model = Matrix4x4.CreateTRS(Vector3.One, Quaternion.FromEulerAngles(0, 0, 0), Vector3.One);
        Matrix4x4 view = frame.Camera.ViewMatrix;
        Matrix4x4 projection = frame.Camera.ProjectionMatrix;

        buffer.SetViewProjection(view, projection);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Pass == "Opaque")
                buffer.DrawRenderable(renderable, model);

        buffer.End();
        Renderer.SubmitCommandBuffer(buffer);
        buffer.Dispose();
    }
}
