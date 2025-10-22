using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Dumpy.Utils;

public static class TypeUtil
{
    private static readonly TypeMemberCache _typeMemberCache = new();
    private static readonly ConcurrentDictionary<Type, string?[]> _typeNameCache = new();
    private static readonly Type _nullableType = typeof(Nullable<>);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStringFormattable(Type type)
    {
        return type.IsPrimitive
               || type == typeof(string)
               || type.IsEnum
               || type.IsNullableOfT()
               || typeof(IFormattable).IsAssignableFrom(type)
               || typeof(Exception).IsAssignableFrom(type)
               || typeof(Type).IsAssignableFrom(type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCollection(Type type)
    {
        return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsObject(Type type)
    {
        return !IsStringFormattable(type) && !IsCollection(type);
    }

    /// <summary>
    /// Returns <see langword="true" /> when the given type is of type <see cref="Nullable{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullableOfT(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == _nullableType;

    /// <summary>
    /// Returns <see langword="true" /> when the given type is assignable from <paramref name="from"/> including support
    /// when <paramref name="from"/> is <see cref="Nullable{T}"/> by using the {T} generic parameter for <paramref name="from"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAssignableFromInternal(this Type type, Type from)
    {
        if (IsNullableOfT(from) && type.IsInterface)
        {
            return type.IsAssignableFrom(from.GetGenericArguments()[0]);
        }

        return type.IsAssignableFrom(from);
    }

    public static string GetName(Type type, bool fullyQualify = false)
    {
        int nameIndex = fullyQualify ? 0 : 1;

        if (_typeNameCache.TryGetValue(type, out var cached) && cached[nameIndex] != null)
        {
            return cached[nameIndex]!;
        }

        var cache = _typeNameCache.GetOrAdd(type, static _ => [null, null]);
        var name = fullyQualify ? type.FullName ?? type.Name : type.Name;

        if (!type.IsGenericType)
        {
            cache[nameIndex] = name;
            return name;
        }

        var sb = new StringBuilder();

        if (type.Namespace == null && name.Contains("AnonymousType"))
        {
            sb.Append("AnonymousType");
        }
        else
        {
            sb.Append(name.AsSpan(0, name.IndexOf('`')));
        }

        sb.Append(type
            .GetGenericArguments()
            .Aggregate("<",
                delegate(string aggregate, Type argType)
                {
                    return aggregate + (aggregate == "<" ? "" : ",") + GetName(argType, fullyQualify);
                }
            ));

        sb.Append('>');

        var result = sb.ToString();
        cache[nameIndex] = result;
        return result;
    }

    public static PropertyInfo[] GetReadableProperties(Type type, bool includeNonPublic)
    {
        var members = _typeMemberCache.GetMembers(type, includeNonPublic);
        if (members.Properties != null)
        {
            return members.Properties;
        }

        var bindingFlags = includeNonPublic
            ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            : BindingFlags.Instance | BindingFlags.Public;

        members.Properties = type
            .GetProperties(bindingFlags)
            // Only include readable properties, and exclude indexer properties
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            // Exclude properties that exist in base types and are hidden by properties in derived types
            .GroupBy(p => p.Name)
            .Select(g => g.OrderBy(p => p.DeclaringType == type).First())
            .ToArray();

        return members.Properties;
    }

    public static FieldInfo[] GetFields(Type type, bool includeNonPublic)
    {
        var members = _typeMemberCache.GetMembers(type, includeNonPublic);
        if (members.Fields != null)
        {
            return members.Fields;
        }

        var bindingFlags = includeNonPublic
            ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            : BindingFlags.Instance | BindingFlags.Public;

        members.Fields = type.GetFields(bindingFlags)
            .GroupBy(f => f.Name)
            .Select(g => g.OrderBy(p => p.DeclaringType == type).First())
            .ToArray();

        return members.Fields;
    }

    public static (Type memberType, object? value) GetMemberTypeAndValue(this MemberInfo member, object? obj)
    {
        Type memberType;
        object? value;

        switch (member)
        {
            case PropertyInfo property:
                memberType = property.PropertyType;
                value = GetPropertyValue(property, obj);
                break;
            case FieldInfo field:
                memberType = field.FieldType;
                value = GetFieldValue(field, obj);
                break;
            default:
                throw new InvalidOperationException($"Unexpected member type: {member.MemberType}");
        }

        return (memberType, value);
    }

    public static object? GetFieldValue<T>(FieldInfo field, T obj)
    {
        try
        {
            return field.GetValue(obj);
        }
        catch
        {
            return string.Empty;
        }
    }

    public static object? GetPropertyValue<T>(PropertyInfo property, T obj)
    {
        //return PropertyAccessor.GetPropertyValue(obj, property);

        try
        {
            // See for alternatives:
            // https://stackoverflow.com/questions/4939508/get-value-of-c-sharp-dynamic-property-via-string
            // https://stackoverflow.com/questions/17660097/is-it-possible-to-speed-this-method-up/17669142#17669142
            // https://stackoverflow.com/questions/23150027/how-would-i-get-all-public-properties-of-object-faster-than-with-propertyinfo-ge
            return property.GetValue(obj);
        }
        catch
        {
            return string.Empty;
        }
    }

    public static Type? GetCollectionElementType(Type collectionType)
    {
        // Arrays
        if (collectionType.IsArray)
        {
            return collectionType.GetElementType();
        }

        // IEnumerable<T> collections
        Type? iEnumerable = FindIEnumerable(collectionType);
        if (iEnumerable != null)
        {
            return iEnumerable.GetGenericArguments()[0];
        }

        // Collections that might have an indexer
        var indexerItemType = collectionType.GetProperties()
            .FirstOrDefault(p => p.GetIndexParameters().Length > 0 && p.PropertyType != typeof(object))?.PropertyType;

        return indexerItemType ?? typeof(object);
    }

    private static Type? FindIEnumerable(Type collectionType)
    {
        if (collectionType == typeof(string))
        {
            return null;
        }

        if (collectionType.IsGenericType)
        {
            foreach (Type arg in collectionType.GetGenericArguments())
            {
                Type iEnumerable = typeof(IEnumerable<>).MakeGenericType(arg);
                if (iEnumerable.IsAssignableFrom(collectionType))
                {
                    return iEnumerable;
                }
            }
        }

        foreach (Type iFace in collectionType.GetInterfaces())
        {
            Type? iEnumerable = FindIEnumerable(iFace);
            if (iEnumerable != null) return iEnumerable;
        }

        if (collectionType.BaseType != null && collectionType.BaseType != typeof(object))
        {
            return FindIEnumerable(collectionType.BaseType);
        }

        return null;
    }
}
