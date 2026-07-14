// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering;

/// <summary>
/// Base class for managed objects that can be shared via renting and returning.
/// </summary>
/// <typeparam name="TObject">The object this pool manages.</typeparam>
public abstract class ObjectPool<TSingletonOwner, TObject>  where TSingletonOwner : new() where TObject : class
{
    /// <summary> Singleton instance. </summary>
	public static TSingletonOwner Instance { get; } = new TSingletonOwner();

    private readonly Queue<TObject> pool = new();

    /// <summary> Creates a new instance of <typeparamref name="TObject"/>. </summary>
    public abstract TObject CreateObject();

    /// <summary> Borrows a <typeparamref name="TObject"/> from the pool or creates a new one if pool has none free. </summary>
    /// <remarks> <typeparamref name="TObject"/>s obtained via this method should be returned via <see cref="Return"/> when no longer in use. </remarks>
    public TObject Rent()
    {
        if (pool.Count > 0)
            return pool.Dequeue();

        return CreateObject();
    }

    /// <summary> Returns the given <typeparamref name="TObject"/> to the pool. </summary>
    public void Return(TObject obj) => pool.Enqueue(obj);
}
