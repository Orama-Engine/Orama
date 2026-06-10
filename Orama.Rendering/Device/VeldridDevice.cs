using Orama.Rendering.Device;
using Orama.Rendering.Resources;
using Silk.NET.Windowing;
using System.Numerics;
using Veldrid;
using Veldrid.OpenGL;

namespace Orama.Rendering.Veldrid;

/// <summary>
/// Interface into low-level Veldrid rendering.
/// </summary>
public class VeldridDevice
{
    /// <summary> The underlying Veldrid <see cref="global::Veldrid.GraphicsDevice"/>. </summary>
    public GraphicsDevice GraphicsDevice { get; private set; } = null!;

    public ulong CurrentFrame { get; internal set; }

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
            SyncToVerticalBlank = false
        };

        switch(backend)
        {
            case RendererBackend.OpenGL:
                var glPlatformInfo = new OpenGLPlatformInfo(
                    openGLContextHandle: window.GLContext!.Handle,
                    getProcAddress: name =>
                    {
                        window.GLContext!.TryGetProcAddress(name, out var ptr);
                        return ptr;
                    },
                    makeCurrent: handle => window.GLContext.MakeCurrent(),
                    getCurrentContext: () => window.GLContext.Handle,
                    clearCurrentContext: () => window.GLContext.Clear(),
                    deleteContext: handle => window.GLContext.Dispose(),
                    swapBuffers: () => window.GLContext.SwapBuffers(),
                    setSyncToVerticalBlank: vsync => window.GLContext.SwapInterval(vsync ? 1 : 0)
                );

                GraphicsDevice = GraphicsDevice.CreateOpenGL(
                    options,
                    glPlatformInfo,
                    (uint)window.Size.X,
                    (uint)window.Size.Y
                );
                break;
            case RendererBackend.Vulkan:
            case RendererBackend.DirectX11:
                {
#warning Vulkan should support Non-Win32
                    if (native is null)
                        throw new NullReferenceException(nameof(native));

                    SwapchainSource source = SwapchainSource.CreateWin32(native.Win32!.Value.Hwnd, native.Win32!.Value.HInstance);

                    SwapchainDescription desc = new(source, (uint)window.Size.X, (uint)window.Size.Y, null, window.VSync);
                    GraphicsDevice = backend == RendererBackend.Vulkan ? GraphicsDevice.CreateVulkan(options, desc) : GraphicsDevice.CreateD3D11(options, desc);
                    break;
                }
        }
    }

    public void SubmitCommands(CommandBuffer commandBuffer) => GraphicsDevice.SubmitCommands(commandBuffer.CommandList);

    /// <inheritdoc/>
    public void Resize(int width, int height) => GraphicsDevice.MainSwapchain.Resize((uint)width, (uint)height);
}
