// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Collections.Concurrent;

using Orama.Common.Utility;

namespace Orama.Rendering;

/// <summary>
/// Base class for managed objects that can be shared via renting and returning.
/// </summary>
/// <typeparam name="TObject">The object this pool manages.</typeparam>
public abstract class ObjectPool<TSingletonOwner, TObject> where TSingletonOwner : new() where TObject : class
{
	/// <summary> The maximum size of the pool before overflowing. </summary>
	public const int MAX_POOL_SIZE = 256;

	/// <summary> Singleton instance. </summary>
	public static TSingletonOwner Instance { get; } = new TSingletonOwner();

	private readonly ConcurrentQueue<TObject> pool = new();

	/// <summary> Creates a new instance of <typeparamref name="TObject"/>. </summary>
	public abstract TObject CreateObject();

	/// <summary> Resets the given <typeparamref name="TObject"/> to it's default state for reuse. </summary>
	public virtual void ResetObject(TObject obj) { }

	/// <summary> Borrows a <typeparamref name="TObject"/> from the pool or creates a new one if pool has none free. </summary>
	/// <remarks> <typeparamref name="TObject"/>s obtained via this method should be returned via <see cref="Return"/> when no longer in use. </remarks>
	public TObject Rent()
	{
		if (!pool.TryDequeue(out TObject? obj))
			obj = CreateObject();

		ResetObject(obj);
		return obj;
	}

	/// <summary> Returns the given <typeparamref name="TObject"/> to the pool. </summary>
	public void Return(TObject obj)
	{
		if (obj == null)
			return;

		if (pool.Count < MAX_POOL_SIZE)
			pool.Enqueue(obj);
		else
			OramaConsole.Warning($"Object Pool for {typeof(TObject).Name} is full! Discarding instance.");
	}
}
