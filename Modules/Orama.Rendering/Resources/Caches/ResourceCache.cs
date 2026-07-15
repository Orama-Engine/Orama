// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Resources.Caches;

/// <summary>
/// Base class for resource caches.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TResource">The type of the resource.</typeparam>
public abstract class ResourceCache<TSingletonOwner, TKey, TResource> where TSingletonOwner : new() where TKey : notnull where TResource : IDisposable
{
	/// <summary> Singleton instance. </summary>
	public static TSingletonOwner Instance { get; } = new TSingletonOwner();

	public Dictionary<TKey, FrameCountedResource<TResource>> Cache { get; } = new();

	/// <summary> Creates a new <typeparamref name="TResource"/> for the given key. </summary>
	protected abstract TResource Create(TKey key);

	/// <summary> Gets or creates a <typeparamref name="TResource"/> for the given key. </summary>
	public FrameCountedResource<TResource> GetOrCreate(TKey key)
	{
		if (Cache.TryGetValue(key, out FrameCountedResource<TResource>? existing))
		{
			existing.Touch();
			return existing;
		}

		TResource created = Create(key);

		FrameCountedResource<TResource> value = new FrameCountedResource<TResource>(created);
		value.Touch();
		value.Disposed += () => Cache.Remove(key);

		Cache[key] = value;

		return value;
	}
}
