

namespace Orama.Rendering.Resources;

public interface IFrameCountedResource
{
    /// <summary> Called when the resource is disposed. </summary>
    public event Action? Disposed;

    /// <summary> Tries to dispose the resource. </summary>
    /// <param name="currentFrame">The current frame. </param>
    /// <returns> True if the resource was disposed. </returns>
    public bool TryDispose(ulong currentFrame);

    /// <summary> Force releases the GPU resource. </summary>
    public void ReleaseGPUResource();
}

/// <summary>
/// GPU Resource that is automatically disposed after it goes unused for a specified number of frames.
/// </summary>
/// <typeparam name="T">The underlying resource type.</typeparam>
public class FrameCountedResource<T> : IFrameCountedResource where T : IDisposable
{
    /// <summary> The number of frames to wait before disposing the resource. Defaults to 200. </summary>
    public uint FrameDisposalBuffer { get; init; } = 200;

    /// <summary> The last frame the resource was used or <see langword="null"/> if never used. </summary>
    internal ulong? LastUsedFrame { get; set; } = null;

    /// <summary> The underlying resource. </summary>
    public T Resource { get; }

    /// <inheritdoc/>
    public event Action? Disposed;

    /// <summary> Initializes a new instance of the <see cref="FrameCountedResource{T}"/> class. </summary>
    public FrameCountedResource(T resource)
    {
        Resource = resource;
        FrameDisposalQueue.ActiveResources.AddLast(this);
    }

    /// <inheritdoc/>
    public bool TryDispose(ulong currentFrame)
    {
        if (LastUsedFrame is null)
            return false;

        if (currentFrame - LastUsedFrame.Value > FrameDisposalBuffer)
        {
            Console.WriteLine($"Disposing {typeof(T).Name} ({Resource})");
            FrameDisposalQueue.DisposalQueue.Enqueue(this);
            Disposed?.Invoke();
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public void ReleaseGPUResource() => Resource.Dispose();
}
