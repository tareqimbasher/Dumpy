using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        var vsb = new ValueStringBuilder(stackalloc char[128]);

        if (type.Namespace == null && name.Contains("AnonymousType"))
        {
            vsb.Append("AnonymousType");
        }
        else
        {
            vsb.Append(name.AsSpan(0, name.IndexOf('`')));
        }

        vsb.Append('<');
        var args = type.GetGenericArguments();
        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0) vsb.Append(',');
            vsb.Append(GetName(args[i], fullyQualify));
        }

        vsb.Append('>');

        var result = vsb.ToString();
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

        var allProps = type.GetProperties(bindingFlags);
        var seen = new Dictionary<string, PropertyInfo>(allProps.Length);
        foreach (var p in allProps)
        {
            // Only include readable properties, and exclude indexer properties
            if (!p.CanRead || p.GetIndexParameters().Length > 0)
            {
                continue;
            }

            // Prefer the derived type's property over inherited ones with the same name
            if (!seen.ContainsKey(p.Name) || p.DeclaringType == type)
            {
                seen[p.Name] = p;
            }
        }

        var result = new PropertyInfo[seen.Count];
        seen.Values.CopyTo(result, 0);
        members.Properties = result;
        return result;
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

        var allFields = type.GetFields(bindingFlags);
        var seen = new Dictionary<string, FieldInfo>(allFields.Length);
        foreach (var f in allFields)
        {
            // Prefer the derived type's field over inherited ones with the same name
            if (!seen.ContainsKey(f.Name) || f.DeclaringType == type)
            {
                seen[f.Name] = f;
            }
        }

        var result = new FieldInfo[seen.Count];
        seen.Values.CopyTo(result, 0);
        members.Fields = result;
        return result;
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
        catch (Exception)
        {
            return DumpError.Instance;
        }
    }

    public static object? GetPropertyValue<T>(PropertyInfo property, T obj)
    {
        try
        {
            return property.GetValue(obj);
        }
        catch (Exception)
        {
            return DumpError.Instance;
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
        foreach (var p in collectionType.GetProperties())
        {
            if (p.GetIndexParameters().Length > 0 && p.PropertyType != typeof(object))
            {
                return p.PropertyType;
            }
        }

        return typeof(object);
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