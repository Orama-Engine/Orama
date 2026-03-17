
using System.Reflection;

namespace Orama.Serialization.Conversion;

/// <summary>
/// Marks a class as a string converter.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class StringConverterAttribute : Attribute
{
    /// <summary> The type of the converter. </summary>
    public Type Type { get; }

    /// <summary> Initializes a new instance of <see cref="StringConverterAttribute"/>. </summary>
    public StringConverterAttribute(Type type) => Type = type;
}

/// <summary>
/// A Class that converts an object to and from a string for serialization.
/// </summary>
internal abstract class StringConverter<T>
{
    private static Dictionary<Type, object>? converters;

    /// <summary> Converts an object to a string. </summary>
    public abstract string ConvertToString(T value);

    /// <summary> Converts a string to an object. </summary>
    public abstract T ConvertFromString(string value);

    /// <summary> Gets the string converter for a type. </summary>
    public static StringConverter<T> Get<T>()
    {
        if (converters is null)
        {
            converters = new Dictionary<Type, object>();
            foreach (var type in typeof(StringConverterAttribute).Assembly.GetTypes())
            {
                var attribute = type.GetCustomAttribute<StringConverterAttribute>();
                if (attribute is not null)
                    converters.Add(attribute.Type, Activator.CreateInstance(type)!);
            }
        }

        return (StringConverter<T>)converters[typeof(T)];
    }
}
