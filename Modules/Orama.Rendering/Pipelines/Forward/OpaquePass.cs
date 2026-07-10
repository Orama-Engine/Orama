using Orama.Common;
using Orama.Common.Utility;
using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Device;
using Orama.Rendering.Resources;

namespace Orama.Rendering.Pipelines.Forward;

/// <summary>
/// Pass responsible for rendering all solid objects.
/// </summary>
public class OpaquePass : RenderPass
{
    /// <inheritdoc/>
    public override void Render(in RenderFrame frame)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;
        var buffer = CommandBufferPool.Rent();

        buffer.Begin();

        buffer.CommandList.SetFramebuffer(gd.SwapchainFramebuffer);

        Matrix4x4 view = frame.View;
        Matrix4x4 projection = frame.Projection;

        GPUBuffer cameraBuffer = new GPUBuffer();
        cameraBuffer.AddMatrix4x4(view);
        cameraBuffer.AddMatrix4x4(projection);
        buffer.QueueGPUBuffer(cameraBuffer, "Camera");

        buffer.ClearColor(Color.Black);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Shader.Pass == "Opaque")
            {
                buffer.QueueGPUBuffer(GetParameterBuffer(renderable), "Parameters");

                Matrix4x4 model = renderable.Transform;

                GPUBuffer objectBuffer = new GPUBuffer();
                objectBuffer.AddMatrix4x4(model);
                buffer.QueueGPUBuffer(objectBuffer, "Object");

                buffer.DrawRenderable(renderable);
            }

        buffer.End();
        Renderer.SubmitCommandBuffer(buffer);
        CommandBufferPool.Return(buffer);
    }

    public static GPUBuffer GetParameterBuffer(IClientRenderable renderable)
    {
        GPUBuffer buffer = new();

        foreach (var param in renderable.Material.Shader.Parameters)
            switch (param)
            {
                case { Type: ShaderParameter.ParamType.Float, DefaultValue: float f }:
                    buffer.AddFloat(f);
                    break;

                case { Type: ShaderParameter.ParamType.Int, DefaultValue: long i }:
                    buffer.AddInt((int)i);
                    break;

                case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector3 v }:
                    buffer.AddFloat3(v.X, v.Y, v.Z);
                    break;

                case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector2 v }:
                    buffer.AddFloat2(v.X, v.Y);
                    break;

                case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector4 v }:
                    buffer.AddFloat4(v.X, v.Y, v.Z, v.W);
                    break;

                default:
                    EngineConsole.Warning($"Unsupported parameter type: {param.Type}");
                    break;
            }

        return buffer;
    }
}
