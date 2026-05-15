namespace Orama.Rendering.Resources.Caches;

/// <summary>
/// Base class for resource caches.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TResource">The type of the resource.</typeparam>
public abstract class ResourceCache<TSingletonOwner, TKey, TResource> where TSingletonOwner : ResourceCache<TSingletonOwner, TKey, TResource>, new() where TKey : notnull
{
    /// <summary> Singleton instance. </summary>
    public static TSingletonOwner Instance { get; } = new TSingletonOwner();

    public Dictionary<TKey, TResource> Cache { get; } = new();

    /// <summary> Creates a new <typeparamref name="TResource"/> for the given key. </summary>
    protected abstract TResource Create(TKey key);

    /// <summary> Gets or creates a <typeparamref name="TResource"/> for the given key. </summary>
    public TResource GetOrCreate(TKey key)
    {
        if (Cache.TryGetValue(key, out TResource? existing))
            return existing;

        TResource created = Create(key);
        Cache[key] = created;
        return created;
    }

    public bool Invalidate(TKey key)
    {
        if (Cache.Remove(key, out TResource? resource))
        {
            (resource as IDisposable)?.Dispose();
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        foreach (var resource in Cache.Values)
            (resource as IDisposable)?.Dispose();

        Cache.Clear();
    }
}