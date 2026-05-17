using System;
using System.Collections.Generic;
using System.Text;

namespace Orama.Rendering.Resources;

/// <summary>
/// Disposal queue for GPU resources.
/// </summary>
internal static class FrameDisposalQueue
{
    public static LinkedList<IFrameCountedResource> ActiveResources { get; } = new LinkedList<IFrameCountedResource>();
    public static Queue<IFrameCountedResource> DisposalQueue { get; } = new Queue<IFrameCountedResource>();
}
