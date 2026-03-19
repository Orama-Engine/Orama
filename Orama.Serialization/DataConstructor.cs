using Orama.Serialization.Conversion;
using System.Reflection;

namespace Orama.Serialization;

/// <summary>
/// Class responsible for converting an instance to a <see cref="InstanceRepresentation"/>.
/// </summary>
internal static class DataConstructor
{
    private static HashSet<Type> currentlyConstructing = new();

    /// <summary> Constructs a <see cref="InstanceRepresentation"/> from an instance. </summary>
    public static InstanceRepresentation Construct<T>(T instance) => new() { Fields = ConstructFields(instance!, null, new HashSet<object>(ReferenceEqualityComparer.Instance)) };

    /// <summary> Deconstructs a <see cref="InstanceRepresentation"/> to an instance. </summary>
    public static T? Deconstruct<T>(InstanceRepresentation rep) => (T?)DeconstructObject(typeof(T), rep.Fields);

    private static FieldRepresentation[] ConstructFields(object instance, string? prefix, HashSet<object> visited)
    {
        // Structs are value types, only track reference types
        if (!instance.GetType().IsValueType)
        {
            if (!visited.Add(instance))
                return Array.Empty<FieldRepresentation>();
        }

        var type = instance.GetType();
        var fields = new List<FieldRepresentation>();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanWrite && p.CanRead))
        {
            var value = prop.GetValue(instance);
            var fullName = prefix == null ? prop.Name : $"{prefix}.{prop.Name}";

            if (value == null)
            {
                fields.Add(new FieldRepresentation(fullName, ""));
                continue;
            }

            if (TryConvertToString(prop.PropertyType, value, out var stringValue))
            {
                fields.Add(new FieldRepresentation(fullName, stringValue!));
            }
            else if (IsNestedObject(prop.PropertyType))
            {
                fields.AddRange(ConstructFields(value, fullName, visited));
            }
            else
            {
                throw new Exception($"No string converter found for type {prop.PropertyType.Name}.");
            }
        }

        return fields.ToArray();
    }

    private static bool TryConvertToString(Type type, object value, out string? result)
    {
        try
        {
            var converter = StringConverterAttribute.GetConverter(type);
            var method = converter.GetType().GetMethod("ConvertToString")!;
            result = (string)method.Invoke(converter, new[] { value })!;
            return true;
        }
        catch { result = null; return false; }
    }

    private static object? DeconstructObject(Type type, FieldRepresentation[] fields)
    {
        var instance = Activator.CreateInstance(type);

        // Separate flat fields from dotted (nested) ones
        var flat = fields.Where(f => !f.Name.Contains('.')).ToArray();
        var nested = fields.Where(f => f.Name.Contains('.')).GroupBy(f => f.Name.Split('.')[0]).ToDictionary(g => g.Key, g => g.Select(f => new FieldRepresentation(f.Name[(f.Name.IndexOf('.') + 1)..], f.Value)).ToArray());

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            // Flat primitive field
            if (flat.FirstOrDefault(f => f.Name == prop.Name) is { Name: not null } field)
            {
                if (string.IsNullOrEmpty(field.Value)) continue;

                try
                {
                    var converter = StringConverterAttribute.GetConverter(prop.PropertyType);
                    var method = converter.GetType().GetMethod("ConvertFromString")!;
                    prop.SetValue(instance, method.Invoke(converter, new object[] { field.Value }));
                }
                catch
                {
                    throw new Exception($"No string converter found for type {prop.PropertyType.Name}.");
                }
            }
            else if (nested.TryGetValue(prop.Name, out var childFields))
            {
                prop.SetValue(instance, DeconstructObject(prop.PropertyType, childFields));
            }
        }

        return instance;
    }

    private static bool IsNestedObject(Type type) => !type.IsPrimitive && type != typeof(string) && !type.IsEnum && type.GetProperties().Length > 0;
}
