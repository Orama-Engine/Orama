// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.RHI.Resources;
using Silk.NET.Windowing;
using Silk.NET.Vulkan;
using Silk.NET.Core.Native;
using Orama.Common.Utility;

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

	/// <summary> The underlying Vulkan <see cref="Silk.NET.Vulkan.SurfaceKHR"/>. </summary>
	public SurfaceKHR Surface { get; private set; }

	/// <summary> Index that points to the Queue Family responsible for graphics operations. </summary>
	public uint GraphicsQueueFamilyIndex { get; private set; }

	/// <summary> Index that points to the Queue Family responsible for compute operations. </summary>
	public uint ComputeQueueFamilyIndex { get; private set; }

	/// <inheritdoc/>
	public VulkanInfo? VulkanInfo
	{
		get
		{
			VulkanInfo info = new()
			{
				Device = Device.Handle,
				PhysicalDevice = PhysicalDevice.Handle,
				GraphicsQueueFamilyIndex = GraphicsQueueFamilyIndex,
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
			if (window.VkSurface == null)
				throw new VulkanException("Failed to create Vulkan Surface.", Result.ErrorInitializationFailed);

			ApplicationInfo appInfo = new()
			{
				SType = StructureType.ApplicationInfo,

				PApplicationName = (byte*)appNamePtr,
				PEngineName = (byte*)engineNamePtr,

				ApplicationVersion = Vk.MakeVersion(1, 0),
				EngineVersion = Vk.MakeVersion(1, 0),
				ApiVersion = Vk.Version13
			};
			
			// TODO: Custom extensions
			byte** extensionsPtr = window.VkSurface.GetRequiredExtensions(out uint count);

			InstanceCreateInfo instanceInfo = new()
			{
				SType = StructureType.InstanceCreateInfo,
				PApplicationInfo = &appInfo,
				EnabledExtensionCount = count,
				PpEnabledExtensionNames = extensionsPtr
			};

			Instance instance;
			Vk.CreateInstance(&instanceInfo, null, &instance).ThrowIfFailed("Failed to create Vulkan Instance.");
			Instance = instance;

			var surfaceHandle = window.VkSurface.Create<AllocationCallbacks>(instance.ToHandle(), null);
			Surface = surfaceHandle.ToSurface();

			PhysicalDevice = Vk.GetOptimalPhysicalDevice(instance);

			ReadOnlySpan<QueueFamilyProperties2> queueFamilyProperties = Vk.GetPhysicalDeviceQueueFamilyProperties2Span(PhysicalDevice);
			DeviceQueueCreateInfo* queueCreateInfos = stackalloc DeviceQueueCreateInfo[queueFamilyProperties.Length];

			for (int i = 0; i < queueFamilyProperties.Length; i++)
			{
				if (queueFamilyProperties[i].QueueFamilyProperties.QueueFlags.HasFlag(QueueFlags.GraphicsBit))
					GraphicsQueueFamilyIndex = (uint)i;

				if (queueFamilyProperties[i].QueueFamilyProperties.QueueFlags.HasFlag(QueueFlags.ComputeBit))
					ComputeQueueFamilyIndex = (uint)i;

				float queuePriority = 1f;

				queueCreateInfos[i] = new DeviceQueueCreateInfo()
				{
					SType = StructureType.DeviceQueueCreateInfo,
					QueueFamilyIndex = (uint)i,
					QueueCount = 1,
					PQueuePriorities = &queuePriority
				};
			}

			PhysicalDeviceFeatures deviceFeatures;

			DeviceCreateInfo deviceInfo = new()
			{
				SType = StructureType.DeviceCreateInfo,
				QueueCreateInfoCount = (uint)queueFamilyProperties.Length,
				PQueueCreateInfos = queueCreateInfos,
				PEnabledFeatures = &deviceFeatures,
				EnabledExtensionCount = 0,
				PpEnabledExtensionNames = null
			};

			Device device;
			Vk.CreateDevice(PhysicalDevice, &deviceInfo, null, &device).ThrowIfFailed("Failed to create Vulkan Device.");
			Device = device;

			OramaConsole.Log(Device.Handle.ToString());
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
