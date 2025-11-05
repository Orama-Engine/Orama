using Orama.Core.Common;
using Orama.Rendering;
using System.Numerics;

namespace Orama.Core.Modules.Rendering.Pipelines.Forward;

internal class GUIPass : RenderPass
{
    public override void Render()
    {
        Renderer.CommandBuffer.DisableFeature(Orama.Rendering.Backends.RenderFeature.CullFaces);
        Renderer.CommandBuffer.EnableFeature(Orama.Rendering.Backends.RenderFeature.Blending);
        Renderer.CommandBuffer.EnableFeature(Orama.Rendering.Backends.RenderFeature.DepthTest);
        Renderer.CommandBuffer.SetDepthMask(true);

        foreach (IClientRenderable renderable in ModuleManager.GetModule<RenderingModule>()?.Renderables ?? Enumerable.Empty<IClientRenderable>())
            if (renderable.Material.Pass == "GUI")
                QueueObject(renderable);

        Matrix4x4 ortho = Matrix4x4.CreateOrthographicOffCenter(
            0, Application.Window.Size.X,
            Application.Window.Size.Y, 0,
            0, 1
        );

        Renderer.Render(Matrix4x4.Identity, ortho);
    }
}
