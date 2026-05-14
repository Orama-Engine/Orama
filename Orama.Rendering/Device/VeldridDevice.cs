using Orama.Rendering.Device;
using Orama.Rendering.Resources;
using Silk.NET.Windowing;
using System.Numerics;
using Veldrid;

namespace Orama.Rendering.Veldrid;

/// <summary>
/// Interface into low-level Veldrid rendering.
/// </summary>
public class VeldridDevice
{
    /// <summary> The underlying Veldrid <see cref="global::Veldrid.GraphicsDevice"/>. </summary>
    public GraphicsDevice GraphicsDevice { get; private set; } = null!;

    private RendererBackend backend;

    /// <summary> Initializes a new instance of the <see cref="VeldridDevice"/> class. </summary>
    public VeldridDevice(RendererBackend backend)
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
                GraphicsDevice = GraphicsDevice.CreateVulkan(options, desc);
                break;
            case RendererBackend.DirectX11:
                GraphicsDevice = GraphicsDevice.CreateD3D11(options, desc);
                break;
        }
    }

    public void SubmitCommands(CommandBuffer commandBuffer) => GraphicsDevice.SubmitCommands(commandBuffer.CommandList);

    /// <inheritdoc/>
    public void Resize(int width, int height) => GraphicsDevice.MainSwapchain.Resize((uint)width, (uint)height);
}
