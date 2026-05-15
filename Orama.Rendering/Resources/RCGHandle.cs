namespace Orama.Rendering.Resources;

/// <summary> Immutable reference to a <see cref="RCGResource{T}"/>. </summary>
public readonly struct RCGHandle<T> where T : IDisposable
{
    public static RCGHandle<T> Empty => default;

    public ulong ID { get; }
    public bool IsValid => ID != 0;

    /// <summary> Initializes a new instance of the <see cref="RCGHandle{T}"/> struct. </summary>
    internal RCGHandle(ulong id) => ID = id;

    /// <inheritdoc/>
    public override string ToString() => IsValid ? $"GPUHandle<{typeof(T).Name}>({ID})" : "GPUHandle.Invalid";
}
