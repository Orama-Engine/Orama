// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Runtime.InteropServices;

using Orama.Common.Utility;

namespace Orama.Rendering.Resources;

/// <summary>
/// Disposal queue for GPU resources.
/// </summary>
internal static class FrameDisposalQueue
{
	public static List<IFrameCountedResource> ActiveResources { get; } = new List<IFrameCountedResource>();
	public static Queue<IFrameCountedResource> DisposalQueue { get; } = new Queue<IFrameCountedResource>();

	/// <summary> Releases <see cref="FrameCountedResource{T}"/>s that are safe to dispose. </summary>
	public static void DisposeResources(ulong currentFrame)
	{
		for (int i = ActiveResources.Count - 1; i >= 0; i--)
		{
			var resource = ActiveResources[i];

			if (resource.TryDispose(currentFrame))
			{
				ActiveResources.RemoveAt(i);
				DisposalQueue.Enqueue(resource);
			}
		}

		// TODO: We need to wait for the GPU to finish using the resources before disposing them
		// This is somewhat safe because we wait so many frames before running this but obviously we need to implement actual fencing
		while (DisposalQueue.Count > 0)
			DisposalQueue.Dequeue().ReleaseGPUResource();
	}
}
