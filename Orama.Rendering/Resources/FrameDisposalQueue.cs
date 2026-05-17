
namespace Orama.Rendering.Resources;

/// <summary>
/// Disposal queue for GPU resources.
/// </summary>
internal static class FrameDisposalQueue
{
    public static LinkedList<IFrameCountedResource> ActiveResources { get; } = new LinkedList<IFrameCountedResource>();
    public static Queue<IFrameCountedResource> DisposalQueue { get; } = new Queue<IFrameCountedResource>();

    /// <summary> Releases <see cref="FrameCountedResource{T}"/>s that are safe to dispose. </summary>
    public static void DisposeResources(ulong currentFrame)
    {
        foreach (var resource in ActiveResources.ToArray())
        {
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
