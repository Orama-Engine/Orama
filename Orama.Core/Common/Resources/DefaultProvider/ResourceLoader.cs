
namespace Orama.Core.Common.Resources.DefaultProvider;

/// <summary>
/// Base class for handling of resource loading.
/// </summary>
internal abstract class ResourceLoader<T> where T : class
{
    /// <summary> Loads a resource from the given byte data. </summary>
    /// <param name="data"> The byte data. </param>
    /// <returns> The loaded resource or null if invalid. </returns>
    public abstract T? LoadResource(byte[] data);

    /// <summary> Gets an instance of the resource loader this type. </summary>
    /// <returns> The resource loader for the specified type or null if not found. </returns>
    public static ResourceLoader<T>? GetResourceLoader()
    {
        var loaderType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t =>
                typeof(ResourceLoader<T>).IsAssignableFrom(t) &&
                t.GetCustomAttributes(typeof(ResourceLoaderAttribute), inherit: false).Any());

        if (loaderType == null)
            return null;

        return (ResourceLoader<T>)Activator.CreateInstance(loaderType)!;
    }
}

/// <summary>
/// Marks a class as a resource loader.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class ResourceLoaderAttribute : Attribute { }