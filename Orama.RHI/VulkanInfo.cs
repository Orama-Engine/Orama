// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.RHI;

/// <summary>
/// Low-Level information about a Vulkan instance.
/// </summary>
public readonly struct VulkanInfo
{
    public IntPtr Instance { get; init; }
    public IntPtr PhysicalDevice { get; init; }
    public IntPtr Device { get; init; }

    public uint GraphicsQueueFamilyIndex { get; init; }
}
