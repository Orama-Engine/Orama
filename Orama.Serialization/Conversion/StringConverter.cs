
using System.Reflection;

namespace Orama.Serialization.Conversion;

/// <summary>
/// Marks a class as a string converter.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class StringConverterAttribute : Attribute
{
    private static Dictionary<Type, object>? converters = null;

    /// <summary> The type of the converter. </summary>
    public Type Type { get; }

    /// <summary> Initializes a new instance of <see cref="StringConverterAttribute"/>. </summary>
    public StringConverterAttribute(Type type) => Type = type;

    /// <summary> Gets the string converter for a type. </summary>
    public static object GetConverter(Type type)
    {
        if (converters is null)
        {
            converters = new Dictionary<Type, object>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;

                try { types = assembly.GetTypes(); }
                catch { continue; }

                foreach (var t in types)
                {
                    var attr = t.GetCustomAttribute<StringConverterAttribute>();
                    if (attr != null)
                    {
                        converters[attr.Type] = Activator.CreateInstance(t)!;
                    }
                }
            }
        }

        if (!converters.TryGetValue(type, out var converter))
            throw new Exception($"No string converter found for type {type}.");

        return converter;
    }
}

/// <summary>
/// A Class that converts an object to and from a string for serialization.
/// </summary>
internal abstract class StringConverter<T>
{
    /// <summary> Converts an object to a string. </summary>
    public abstract string ConvertToString(T value);

    /// <summary> Converts a string to an object. </summary>
    public abstract T ConvertFromString(string value);
}
