
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
                var converterType = typeof(StringConverter<>).MakeGenericType(prop.PropertyType);
                var getMethod = converterType.GetMethod("Get", BindingFlags.Public | BindingFlags.Static)!;
                var converter = getMethod.Invoke(null, null)!;

                var convertMethod = converter.GetType().GetMethod("ConvertFromString")!;
                value = convertMethod.Invoke(converter, new object[] { field.Value });
            }
            catch
            {
                value = Convert.ChangeType(field.Value, prop.PropertyType);
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
                var converterType = typeof(StringConverter<>).MakeGenericType(p.PropertyType);
                var getMethod = converterType.GetMethod("Get", BindingFlags.Public | BindingFlags.Static)!;
                var converter = getMethod.Invoke(null, null)!;

                var convertMethod = converter.GetType().GetMethod("ConvertToString")!;
                stringValue = (string)convertMethod.Invoke(converter, new[] { value })!;
            }
            catch
            {
                stringValue = value.ToString()!;
            }

            return new FieldRepresentation(p.Name, stringValue);

        }).ToArray();
    }
}
