// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Silk.NET.Vulkan;

namespace Orama.RHI.VulkanBackend;

/// <summary>
/// Various wrappers and utility functions for Vulkan.
/// </summary>
public unsafe static class VulkanExtensions
{
	extension (Vk vk)
	{
		/// <summary> Returns the optimal <see cref="PhysicalDevice"/> available. </summary>
		public PhysicalDevice GetOptimalPhysicalDevice(Instance instance)
		{
			PhysicalDevice optimal = default;
			foreach (var physDevice in vk.GetPhysicalDevices(instance))
			{
				PhysicalDeviceProperties2 props;
				vk.GetPhysicalDeviceProperties2(physDevice, &props);

				// We obviously need to check more than this
				if (props.Properties.DeviceType == PhysicalDeviceType.DiscreteGpu)
				{
					optimal = physDevice;
					break;
				}
			}

			return optimal;
		}

		/// <summary> Returns a <see cref="ReadOnlySpan{T}"/> containing all available <see cref="QueueFamilyProperties2"/> for the given <see cref="PhysicalDevice"/>. </summary>
		public ReadOnlySpan<QueueFamilyProperties2> GetPhysicalDeviceQueueFamilyPropertiesSpan(PhysicalDevice device)
		{
			uint queueFamilyCount;
			vk.GetPhysicalDeviceQueueFamilyProperties2(device, &queueFamilyCount, null);
			QueueFamilyProperties2[] queueFamilies = new QueueFamilyProperties2[queueFamilyCount];

			vk.GetPhysicalDeviceQueueFamilyProperties2(device, &queueFamilyCount, queueFamilies);

			return queueFamilies;
		}
	}

	extension(Result res)
	{
		/// <summary> Throws a <see cref="VulkanException"/> with the given message if the result is not <see cref="Result.Success"/>. </summary>
		public Result ThrowIfFailed(string exMessage)
		{
			if (res != Result.Success)
				throw new VulkanException(exMessage, res);

			return res;
		}
	}
}
