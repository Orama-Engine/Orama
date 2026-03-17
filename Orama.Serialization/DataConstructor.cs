
using Orama.Serialization.Conversion;
using System.ComponentModel;
using System.Reflection;

namespace Orama.Serialization;

/// <summary>
/// Class responsible for converting an instance to a <see cref="InstanceRepresentation"/>.
/// </summary>
internal static class DataConstructor
{
    /// <summary> Constructs a <see cref="InstanceRepresentation"/> from an instance. </summary>
    public static InstanceRepresentation Construct<T>(T instance)
    {
        FieldRepresentation[] fields = ConstructFields(instance);

        return new InstanceRepresentation { Fields = fields };
    }

    /// <summary> Deconstructs a <see cref="InstanceRepresentation"/> to an instance. </summary>
    public static T? Deconstruct<T>(InstanceRepresentation rep)
    {
        var type = typeof(T);
        var instance = Activator.CreateInstance(type);

        foreach (var field in rep.Fields)
        {
            var prop = type.GetProperty(field.Name);
            if (prop == null || !prop.CanWrite)
                continue;

            object? value = null;

            try
            {
                var converter = StringConverterAttribute.GetConverter(prop.PropertyType);
                var method = converter.GetType().GetMethod("ConvertFromString")!;
                value = method.Invoke(converter, new object[] { field.Value });
            }
            catch
            {
                throw new Exception($"No string converter found for type {prop.PropertyType}.");
            }

            prop.SetValue(instance, value);
        }

        return (T?)instance;
    }

    private static FieldRepresentation[] ConstructFields(object instance)
    {
        var type = instance.GetType();

        return type.GetProperties().Select(p =>
        {
            var value = p.GetValue(instance);

            if (value == null)
                return new FieldRepresentation(p.Name, "");

            string stringValue;

            try
            {
                var converter = StringConverterAttribute.GetConverter(p.PropertyType);
                var method = converter.GetType().GetMethod("ConvertToString")!;
                stringValue = (string)method.Invoke(converter, new[] { value })!;
            }
            catch
            {
                throw new Exception($"No string converter found for type {p.PropertyType}.");
            }

            return new FieldRepresentation(p.Name, stringValue);

        }).ToArray();
    }
}
