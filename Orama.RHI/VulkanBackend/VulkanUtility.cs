// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Silk.NET.Vulkan;

namespace Orama.RHI.VulkanBackend;

/// <summary>
/// Various wrappers and utility functions for Vulkan.
/// </summary>
public static class VulkanUtility
{
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
