// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)


using Silk.NET.Vulkan;

namespace Orama.RHI.VulkanBackend;

/// <summary>
/// Thrown when <see cref="VulkanDevice"/> has an error.
/// </summary>
public class VulkanException : Exception
{
	/// <summary> The result code returned by Vulkan. </summary>
	public Result VulkanResult { get; init; }

	public VulkanException(string message, Result result) : base($"{message} (Error: {result})") => VulkanResult = result;
}
