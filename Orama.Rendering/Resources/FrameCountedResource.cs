

namespace Orama.Rendering.Resources;

public interface IFrameCountedResource
{
    /// <summary> Tries to dispose the resource. </summary>
    /// <param name="currentFrame">The current frame. </param>
    /// <returns> True if the resource was disposed. </returns>
    public bool TryDispose(uint currentFrame);
}

/// <summary>
/// GPU Resource that is automatically disposed after it goes unused for a specified number of frames.
/// </summary>
/// <typeparam name="T">The underlying resource type.</typeparam>
public class FrameCountedResource<T> : IFrameCountedResource where T : IDisposable
{
    /// <summary> The number of frames to wait before disposing the resource. Defaults to 60. </summary>
    public uint FrameDisposalBuffer { get; init; } = 60;

    /// <summary> The last frame the resource was used. </summary>
    internal uint LastUsedFrame { get; set; }

    /// <summary> The underlying resource. </summary>
    public T Resource { get; }

    /// <summary> Initializes a new instance of the <see cref="FrameCountedResource{T}"/> class. </summary>
    public FrameCountedResource(T resource)
    {
        Resource = resource;
        FrameDisposalQueue.ActiveResources.AddLast(this);
    }

    /// <inheritdoc/>
    public bool TryDispose(uint currentFrame)
    {
        if (currentFrame - LastUsedFrame > FrameDisposalBuffer)
        {
            Console.WriteLine($"Disposing GPU Resource {Resource}");
            FrameDisposalQueue.DisposalQueue.Enqueue(this);
            return true;
        }

        return false;
    }
}
