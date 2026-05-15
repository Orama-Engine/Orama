
using System.Reflection.Emit;

namespace Orama.Rendering.Resources;

public enum GPUResourceState : byte
{
    /// <summary> Valid on CPU and GPU side. </summary>
    Live,

    /// <summary> Invalidated on CPU side, waiting for GPU to finish use. </summary>
    PendingDeletion,

    /// <summary> Invalidated on CPU and GPU side. </summary>
    Destroyed
}

/// <summary>
/// Reference Counted GPU Resource.
/// </summary>
public class RCGResource<T> where T : IDisposable
{
    /// <summary> ID/Handle of the resource. </summary>
    public ulong ID { get; }

    /// <summary> String name of the resource. </summary>
    public string Label { get; }

    /// <summary> Current lifecycle state of the resource. </summary>
    public GPUResourceState State { get; private set; } = GPUResourceState.Live;

    /// <summary> Last frame a command buffer that used this resource was submitted. </summary>
    public ulong LastUsedFrame { get; private set; }

    /// <summary> Frame on which Invalidate() was called or <see cref="ulong.MaxValue"/> if never invalidated. </summary>
    public ulong InvalidatedOnFrame { get; private set; } = ulong.MaxValue;

    /// <summary> The amount of current references to the resource. </summary>
    internal int RefCount => Volatile.Read(ref refCount);

    private T? value;
    private int refCount;

    /// <summary> Initializes a new instance of the <see cref="RCGResource{T}"/> class. </summary>
    internal RCGResource(ulong id, string label, T value)
    {
        ID = id;
        Label = label;
        this.value = value;
    }

    // TODO: Could we return a nullable instead of throwing if disposed?
    /// <summary> Returns the underlying GPU object or throws if the resource is destroyed. </summary>
    public T Get()
    {
        ObjectDisposedException.ThrowIf(State == GPUResourceState.Destroyed, Label);
        return value!;
    }

    /// <summary> Returns the underlying GPU object or null if the resource is destroyed. </summary>
    public T GetUnsafe() => value!;

    internal void AddRef()
    {
        Interlocked.Increment(ref refCount);
        Console.WriteLine($"Added ref to {Label}. Current ref count: {refCount}");
    }

    internal bool Release()
    {
        Console.WriteLine($"Releasing ref to {Label}. Current ref count: {refCount - 1}");
        return Interlocked.Decrement(ref refCount) == 0;
    }

    internal void MarkUsed() => LastUsedFrame = Renderer.Veldrid.CurrentFrame;

    /// <summary> Marks the resource as pending deletion when the GPU is done with it. </summary>
    internal void MarkPendingDeletion(ulong frame)
    {
        State = GPUResourceState.PendingDeletion;
        InvalidatedOnFrame = frame;
    }

    /// <summary> Disposes of the resource without any safety checks. </summary>
    internal void ForceDispose()
    {
        State = GPUResourceState.Destroyed;
        value?.Dispose();
        value = default;
    }
}
