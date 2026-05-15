using System;
using System.Collections.Generic;
using System.Text;

namespace Orama.Rendering.Resources;

/// <summary>
/// Manages life cycle for all GPU resources of type <typeparamref name="T"/>.
/// </summary>
public sealed class RCGResourceRegistry<T> where T : IDisposable
{
    public static RCGResourceRegistry<T> Instance {  get; } = new();

    private readonly Dictionary<ulong, RCGResource<T>> resources = new();
    private readonly ReaderWriterLockSlim rwLock = new();
    private ulong nextID = 1;

    public RCGHandle<T> Register(T value, string label = "")
    {
        ulong id = Interlocked.Increment(ref nextID);
        RCGResource<T> resource = new(id, label, value);

        rwLock.EnterWriteLock();
        try
        {
            resources[id] = resource;
        }
        finally
        {
            rwLock.ExitWriteLock();
        }

        return new RCGHandle<T>(id);
    }

    /// <summary> Acquires a resource from the registry. </summary>
    public RCGResource<T>? Acquire(RCGHandle<T> handle)
    {
        rwLock.EnterReadLock();
        try
        {
            if (!resources.TryGetValue(handle.ID, out var r))
                return null;

            if (r.State != GPUResourceState.Live)
                return null;

            r.AddRef();
            return r;
        }
        finally
        {
            rwLock.ExitReadLock();
        }
    }

    public void Release(RCGHandle<T> handle, ulong currentFrame)
    {
        rwLock.EnterReadLock();
        try
        {
            if (!resources.TryGetValue(handle.ID, out var r))
                return;

            bool zeroed = r.Release();
            if (zeroed && r.State == GPUResourceState.PendingDeletion)
                ScheduleDeletion(r, currentFrame);
        }
        finally
        {
            rwLock.ExitReadLock();
        }
    }

    private static void ScheduleDeletion(RCGResource<T> resource, ulong frame) => Console.WriteLine($"Scheduling deletion of {resource.Label} on frame {frame}");
}
