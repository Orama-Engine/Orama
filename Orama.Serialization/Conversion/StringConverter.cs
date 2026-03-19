
using System.Reflection;

namespace Orama.Serialization.Conversion;

/// <summary>
/// Marks a class as a string converter.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StringConverterAttribute : Attribute
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
                        converters[attr.Type] = t;
                }
            }
        }

        if (converters.TryGetValue(type, out var converterType))
            return CreateInstance(converterType, type);

        if (type.IsEnum && converters.TryGetValue(typeof(Enum), out converterType))
            return CreateInstance(converterType, type);

        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (converters.TryGetValue(baseType, out converterType))
                return CreateInstance(converterType, type);
            baseType = baseType.BaseType;
        }

        throw new Exception($"No string converter found for type {type.Name}");
    }

    private static object CreateInstance(object converterTypeOrDefinition, Type targetType)
    {
        var t = (Type)converterTypeOrDefinition;

        if (t.IsGenericTypeDefinition)
            t = t.MakeGenericType(targetType);

        return Activator.CreateInstance(t)!;
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

[StringConverter(typeof(string))]
internal class StringStringConverter : StringConverter<string>
{
    /// <inheritdoc/>
    public override string ConvertToString(string value) => value;

    /// <inheritdoc/>
    public override string ConvertFromString(string value) => value;
}