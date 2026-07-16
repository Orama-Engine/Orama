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
		foreach (var resource in CollectionsMarshal.AsSpan(ActiveResources))
		{
			if (resource == null)
			{
				OramaConsole.Warning($"Tried to dispose FrameCountedResource that was null.");
				continue;
			}

			if (resource.TryDispose(currentFrame))
			{
				ActiveResources.Remove(resource);
				DisposalQueue.Enqueue(resource);
			}
		}

		// TODO: We need to wait for the GPU to finish using the resources before disposing them
		// This is somewhat safe because we wait so many frames before running this but obviously we need to implement actual fencing
		while (DisposalQueue.Count > 0)
		{
			var resource = DisposalQueue.Dequeue();
			resource.ReleaseGPUResource();
		}
	}
}
