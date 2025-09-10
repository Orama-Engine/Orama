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
		return SerializableFieldsCache.GetOrAdd(targetType.TypeHandle, _ =>
		{
			const BindingFlags flags = BindingFlags.Public |
									   BindingFlags.NonPublic |
									   BindingFlags.Instance;

			List<FieldInfo> fields = new List<FieldInfo>();
			Type? currentType = targetType;

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
		// Ignore explicitly ignored or [NonSerialized]
		if (field.GetCustomAttribute<SerializeIgnoreAttribute>() != null ||
			field.GetCustomAttribute<NonSerializedAttribute>() != null)
			return false;

		// Public fields are serialized by default
		if (field.IsPublic)
			return true;

		// Non-public fields require [Serialize] attribute
		return field.GetCustomAttribute<SerializeAttribute>() != null;
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
		// Must be readable/writable
		if (!property.CanRead || !property.CanWrite)
			return false;

		// Ignore explicitly ignored or [NonSerialized]
		if (property.GetCustomAttribute<SerializeIgnoreAttribute>() != null ||
			property.GetCustomAttribute<NonSerializedAttribute>() != null)
			return false;

		// Public properties are serialized by default
		if (property.GetMethod?.IsPublic == true && property.SetMethod?.IsPublic == true)
			return true;

		// Non-public properties require [Serialize] attribute
		return property.GetCustomAttribute<SerializeAttribute>() != null;
	}
}
