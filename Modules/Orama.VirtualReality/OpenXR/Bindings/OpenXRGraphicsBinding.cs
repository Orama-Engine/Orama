// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Runtime.CompilerServices;

using Orama.Rendering;

using Silk.NET.Core.Native;
using Silk.NET.OpenXR;

namespace Orama.VirtualReality.OpenXR.Bindings;

/// <summary>
/// Managed bindings for <see cref="GraphicsBinding"/>.
/// </summary>
internal class OpenXRGraphicsBinding : OpenXRBinding
{
	/// <summary> Pointer to the native graphics binding. </summary>
	public IntPtr Native { get; }

	// We use these as lifetime storage
	private GraphicsBindingVulkanKHR vulkanBinding;
	private GraphicsBindingD3D12KHR d3d12Binding;

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

						Instance = new VkHandle(Renderer.Veldrith.GraphicsDevice.GetVulkanInfo().Instance),
						PhysicalDevice = new VkHandle(Renderer.Veldrith.GraphicsDevice.GetVulkanInfo().PhysicalDevice),
						Device = new VkHandle(Renderer.Veldrith.GraphicsDevice.GetVulkanInfo().Device),

						QueueFamilyIndex = Renderer.Veldrith.GraphicsDevice.GetVulkanInfo().GraphicsQueueFamilyIndex,
						QueueIndex = 0
					};
					Native = (IntPtr)Unsafe.AsPointer(ref vulkanBinding);
					break;

				case RendererBackend.Direct3D12:
					d3d12Binding = new GraphicsBindingD3D12KHR()
					{
						Type = StructureType.GraphicsBindingD3D12Khr,
						Device = (void*)Renderer.Veldrith.GraphicsDevice.GetD3D12Info().Device,
					};
					Native = (IntPtr)Unsafe.AsPointer(ref d3d12Binding);
					break;

				default:
					break;

			}
		}
	}
}
