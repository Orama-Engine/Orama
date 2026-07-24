// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.RHI.Resources;
using Silk.NET.Windowing;
using Silk.NET.Vulkan;
using Silk.NET.Core.Native;

namespace Orama.RHI.VulkanBackend;

/// <summary>
/// Interface into low-level Vulkan rendering.
/// </summary>
internal unsafe sealed class VulkanDevice : IGraphicsDevice
{
	/// <summary> Low-Level Vulkan API. </summary>
	public static Vk Vk => Vk.GetApi();

	/// <summary> The underlying Vulkan <see cref="Silk.NET.Vulkan.Instance"/>. </summary>
	public Instance Instance { get; private set; }

	/// <summary> The underlying Vulkan <see cref="Silk.NET.Vulkan.Device"/>. </summary>
	public Device Device { get; private set; }

	/// <summary> The underlying Vulkan <see cref="Silk.NET.Vulkan.PhysicalDevice"/>. </summary>
	public PhysicalDevice PhysicalDevice { get; private set; }

	/// <inheritdoc/>
	public VulkanInfo? VulkanInfo
	{
		get
		{
			VulkanInfo info = new()
			{
				Device = Device.Handle,
				PhysicalDevice = PhysicalDevice.Handle
			};

			return info;
		}
	}

	/// <inheritdoc/>
	public ulong CurrentFrame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	/// <inheritdoc/>
	public DeviceFeatures Features => throw new NotImplementedException();

	/// <inheritdoc/>
	public IFramebuffer SwapchainFramebuffer => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceFactory ResourceFactory => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Initialize(IWindow window)
	{
		nint appNamePtr = SilkMarshal.StringToPtr(window.Title);
		nint engineNamePtr = SilkMarshal.StringToPtr("Orama");

		try
		{
			ApplicationInfo appInfo = new()
			{
				SType = StructureType.ApplicationInfo,

				PApplicationName = (byte*)appNamePtr,
				PEngineName = (byte*)engineNamePtr,

				ApplicationVersion = Vk.MakeVersion(1, 0),
				EngineVersion = Vk.MakeVersion(1, 0),
				ApiVersion = Vk.Version13
			};

			InstanceCreateInfo instanceInfo = new()
			{
				SType = StructureType.InstanceCreateInfo,
				PApplicationInfo = &appInfo,
			};

			Instance instance;
			Result instanceResult = Vk.CreateInstance(&instanceInfo, null, &instance).ThrowIfFailed("Failed to create Vulkan Instance.");
		}
		finally
		{
			SilkMarshal.Free(appNamePtr);
			SilkMarshal.Free(engineNamePtr);
		}
	}

	/// <inheritdoc/>
	public void Dispose() => throw new NotImplementedException();


	/// <inheritdoc/>
	public void ResizeSwapchain(uint width, uint height) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SubmitCommands(ICommandBuffer buffer) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SwapBuffers() => throw new NotImplementedException();

	/// <inheritdoc/>
	public void UpdateBuffer<T>(IBuffer buffer, uint offset, ReadOnlySpan<T> data) where T : unmanaged => throw new NotImplementedException();
}
