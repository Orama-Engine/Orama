// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Orama.Echo;

[RequiresUnreferencedCode("These methods use reflection and can't be statically analyzed.")]
public static class ReflectionUtils
{
    // Cache for type lookups
    private static readonly ConcurrentDictionary<string, Type?> TypeCache = new();
    // Cache for serializable fields
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, FieldInfo[]> SerializableFieldsCache = new();

    internal static void ClearCache()
    {
        TypeCache.Clear();
        SerializableFieldsCache.Clear();
    }

    internal static Type? FindTypeByName(string qualifiedTypeName)
    {
        return TypeCache.GetOrAdd(qualifiedTypeName, typeName => {
            // First try direct type lookup
            Type? t = Type.GetType(typeName);
            if (t != null)
                return t;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                // Try full name lookup
                t = asm.GetType(typeName);
                if (t != null)
                    return t;
                // Try name-only lookup (case insensitive)
                t = asm.GetTypes().FirstOrDefault(type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
                if (t != null)
                    return t;
            }
            return null;
        });
    }

    internal static FieldInfo[] GetSerializableFields(this object target)
    {
        Type targetType = target.GetType();
        return SerializableFieldsCache.GetOrAdd(targetType.TypeHandle, _ => {
            const BindingFlags flags = BindingFlags.Public |
                                     BindingFlags.NonPublic |
                                     BindingFlags.Instance;

            // Start with the current type
            List<FieldInfo> fields = new List<FieldInfo>();
            Type? currentType = targetType;

            // Walk up the inheritance hierarchy to collect fields from all base types
            while (currentType != null && currentType != typeof(object))
            {
                fields.AddRange(currentType.GetFields(flags)
                    .Where(field => IsFieldSerializable(field)));

                currentType = currentType.BaseType;
            }

            return fields.ToArray();
        });
    }

    private static bool IsFieldSerializable(FieldInfo field)
    {
        // Check if field should be serialized
        bool shouldSerialize = field.GetCustomAttribute<SerializeAttribute>() != null;
        if (!shouldSerialize)
            return false;

        // Check if field should be ignored
        bool shouldIgnore = field.GetCustomAttribute<SerializeIgnoreAttribute>() != null ||
                            field.GetCustomAttribute<NonSerializedAttribute>() != null;
        if (shouldIgnore)
            return false;

        return true;
    }

	internal static PropertyInfo[] GetSerializableProperties(this object target)
	{
		Type targetType = target.GetType();
		return targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(property => IsPropertySerializable(property))
			.ToArray();
	}

	private static bool IsPropertySerializable(PropertyInfo property)
	{
		// Check if property should be serialized
		bool shouldSerialize = property.CanWrite && property.CanRead && property.GetCustomAttribute<SerializeAttribute>() != null;
		if (!shouldSerialize)
			return false;

		// Check if property should be ignored
		bool shouldIgnore = property.GetCustomAttribute<SerializeIgnoreAttribute>() != null ||
							property.GetCustomAttribute<NonSerializedAttribute>() != null;
		if (shouldIgnore)
			return false;

		return true;
	}
}
