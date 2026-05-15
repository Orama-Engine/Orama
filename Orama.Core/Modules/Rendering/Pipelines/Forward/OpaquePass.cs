using Orama.Core.Common;
using Orama.Core.Common.Entities;
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

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Pass == "Opaque")
            {
                Matrix4x4 model = renderable.Transform;
                Matrix4x4 view = frame.Camera.ViewMatrix;
                Matrix4x4 projection = frame.Camera.ProjectionMatrix;

                GPUBuffer paramBuffer = new GPUBuffer();
                paramBuffer.AddMatrix4x4(model);
                paramBuffer.AddMatrix4x4(view);
                paramBuffer.AddMatrix4x4(projection);
                buffer.UploadGPUBuffer(paramBuffer, 0);

                buffer.DrawRenderable(renderable, model);
            }

        buffer.End();
        Renderer.SubmitCommandBuffer(buffer);
        buffer.Dispose();
    }
}
