using Orama.Rendering.Resources;
using Silk.NET.Windowing;
using System.Numerics;
using Veldrid;

namespace Orama.Rendering.Backends.Veldrid;

internal class VeldridBackend : IRendererBackend
{
    /// <inheritdoc/>
    public ICommandBuffer CommandBuffer { get; private set; }

    public GraphicsDevice? Device { get; private set; }

    private RendererBackend backend;

    public VeldridBackend(RendererBackend backend)
    {
        this.backend = backend;
    }

    /// <inheritdoc/>
    public void Initialize(IWindow window)
    {
        var native = window.Native;

        var options = new GraphicsDeviceOptions()
        {
            Debug = true,
            SyncToVerticalBlank = true
        };

#warning Windows only hack
        SwapchainSource source = SwapchainSource.CreateWin32(native.Win32.Value.Hwnd, native.Win32.Value.HInstance);

        SwapchainDescription desc = new SwapchainDescription(source, (uint)window.Size.X, (uint)window.Size.Y, null, true);

        switch(backend)
        {
            case RendererBackend.OpenGL:
                
                break;
            case RendererBackend.Vulkan:
                Device = GraphicsDevice.CreateVulkan(options, desc);
                break;
            case RendererBackend.DirectX11:
                Device = GraphicsDevice.CreateD3D11(options, desc);
                break;
        }

        CommandBuffer = new VeldridBuffer(this);
    }

    /// <inheritdoc/>
    public void Dispose()
    {

    }

    /// <inheritdoc/>
    public void Render(Queue<GraphicsMesh> renderQueue, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, GraphicsTexture? renderTarget = null)
    {

    }

    /// <inheritdoc/>
    public void Resize(int width, int height) => Device.MainSwapchain.Resize((uint)width, (uint)height);
}
