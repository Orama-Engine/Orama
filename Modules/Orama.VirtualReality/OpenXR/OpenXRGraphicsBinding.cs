using Orama.Rendering;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;
using System.Runtime.CompilerServices;

namespace Orama.VirtualReality.OpenXR;

/// <summary>
/// Managed bindings for <see cref="GraphicsBinding"/>.
/// </summary>
internal class OpenXRGraphicsBinding : OpenXRBinding
{
    /// <summary> Pointer to the native graphics binding. </summary>
    public IntPtr Native { get; }

    // We use these as lifetime storage
    private GraphicsBindingVulkanKHR vulkanBinding;
    private GraphicsBindingD3D11KHR d3d11Binding;

    public OpenXRGraphicsBinding(XR openXR, RendererBackend target) : base(openXR)
    {
        unsafe
        {
            switch (target)
            {
                case RendererBackend.Vulkan:
                    vulkanBinding = new GraphicsBindingVulkanKHR
                    {
                        Type = StructureType.GraphicsBindingVulkanKhr,

                        Instance = new VkHandle(Renderer.Veldrid.GraphicsDevice.GetVulkanInfo().Instance),
                        PhysicalDevice = new VkHandle(Renderer.Veldrid.GraphicsDevice.GetVulkanInfo().PhysicalDevice),
                        Device = new VkHandle(Renderer.Veldrid.GraphicsDevice.GetVulkanInfo().Device),

                        QueueFamilyIndex = Renderer.Veldrid.GraphicsDevice.GetVulkanInfo().GraphicsQueueFamilyIndex,
                        QueueIndex = 0
                    };
                    Native = (IntPtr)Unsafe.AsPointer(ref vulkanBinding);
                    break;

                case RendererBackend.Direct3D11:
                    d3d11Binding = new GraphicsBindingD3D11KHR()
                    {
                        Type = StructureType.GraphicsBindingD3D11Khr,
                        Device = (void*)Renderer.Veldrid.GraphicsDevice.GetD3D11Info().Device,
                    };
                    Native = (IntPtr)Unsafe.AsPointer(ref d3d11Binding);
                    break;

                default:
                    break;

            }
        }
    }
}
