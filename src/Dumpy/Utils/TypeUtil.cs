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
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyInfoCache = new();
    private static readonly ConcurrentDictionary<Type, FieldInfo[]> TypeFieldInfoCache = new();
    private static readonly Type NullableType = typeof(Nullable<>);

    public static bool IsStringFormattable(Type type)
    {
        return type.IsPrimitive
               || type == typeof(string)
               || type.IsEnum
               || typeof(IFormattable).IsAssignableFrom(type)
               || typeof(Exception).IsAssignableFrom(type)
               || typeof(Type).IsAssignableFrom(type)
               || Nullable.GetUnderlyingType(type) != null;
    }

    public static bool IsCollection(Type type)
    {
        return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
    }
    
    public static bool IsObject(Type type)
    {
        return !IsStringFormattable(type) && !IsCollection(type);
    }

    /// <summary>
    /// Returns <see langword="true" /> when the given type is of type <see cref="Nullable{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullableOfT(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == NullableType;
    
    /// <summary>
    /// Returns <see langword="true" /> when the given type is assignable from <paramref name="from"/> including support
    /// when <paramref name="from"/> is <see cref="Nullable{T}"/> by using the {T} generic parameter for <paramref name="from"/>.
    /// </summary>
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
        var name = fullyQualify ? type.FullName ?? type.Name : type.Name;

        if (!type.IsGenericType)
        {
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

        return sb.ToString();
    }

    public static PropertyInfo[] GetReadableProperties(Type type, bool includeNonPublic)
    {
        // TODO make includeNonPublic part of key
        if (TypePropertyInfoCache.TryGetValue(type, out var properties))
        {
            return properties;
        }

        var bindingFlags = includeNonPublic
            ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            : BindingFlags.Instance | BindingFlags.Public;

        properties = type
            .GetProperties(bindingFlags)
            // Only include readable properties, and exclude indexer properties
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            // Exclude properties that exist in base types and are hidden by properties in derived types
            .GroupBy(p => p.Name)
            .Select(g => g.OrderBy(p => p.DeclaringType == type).First())
            .ToArray();

        TypePropertyInfoCache.TryAdd(type, properties);
        return properties;
    }

    public static FieldInfo[] GetFields(Type type, bool includeNonPublic)
    {
        if (TypeFieldInfoCache.TryGetValue(type, out var fields))
        {
            return fields;
        }

        var bindingFlags = includeNonPublic
            ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            : BindingFlags.Instance | BindingFlags.Public;
        
        fields = type.GetFields(bindingFlags)
            .GroupBy(f => f.Name)
            .Select(g => g.OrderBy(p => p.DeclaringType == type).First())
            .ToArray();

        TypeFieldInfoCache.TryAdd(type, fields);
        return fields;
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
        var indexerItemType = collectionType.GetProperties().FirstOrDefault(p => p.GetIndexParameters().Length > 0 && p.PropertyType != typeof(object))?.PropertyType;

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

        Type[] interfaces = collectionType.GetInterfaces();

        foreach (Type iFace in interfaces)
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

// public static class PropertyAccessor
// {
//     // Cache for reference type properties (Func<object, object> delegates)
//     private static readonly ConcurrentDictionary<(Type, string), Func<object, object>> _referenceTypeGetterCache =
//         new ConcurrentDictionary<(Type, string), Func<object, object>>();
//
//     // Cache for value type properties (strongly-typed delegates)
//     private static readonly ConcurrentDictionary<(Type, string), Delegate> _valueTypeGetterCache =
//         new ConcurrentDictionary<(Type, string), Delegate>();
//
//     // Method to get the value of a property from an object, optimized for both reference and value types.
//     public static object GetPropertyValue<T>(T target, PropertyInfo property)
//     {
//         var targetType = target.GetType();
//
//         if (!property.CanRead)
//             throw new ArgumentException($"Property '{property.Name}' does not have a getter.");
//
//         if (property.PropertyType.IsValueType)
//         {
//             // For value types, use a strongly-typed delegate to avoid boxing.
//             return GetValueTypePropertyValue(target, targetType, property);
//         }
//         else
//         {
//             // For reference types, use cached Func<object, object> delegate.
//             return GetReferenceTypePropertyValue(target, targetType, property);
//         }
//     }
//
//     // Handles getting reference type property values using cached delegates.
//     private static object GetReferenceTypePropertyValue<T>(T target, Type targetType, PropertyInfo property)
//     {
//         var key = (targetType, property.Name);
//         if (!_referenceTypeGetterCache.TryGetValue(key, out var getter))
//         {
//             getter = CreateReferenceTypeGetterDelegate(property);
//             _referenceTypeGetterCache[key] = getter;
//         }
//
//         // Invoke the cached delegate for reference type.
//         return getter(target);
//     }
//
//     // Handles getting value type property values using strongly-typed delegates (no boxing).
//     private static object GetValueTypePropertyValue<T>(T target, Type targetType, PropertyInfo property)
//     {
//         var key = (targetType, property.Name);
//         if (!_valueTypeGetterCache.TryGetValue(key, out var getter))
//         {
//             getter = CreateValueTypeGetterDelegate(property);
//             _valueTypeGetterCache[key] = getter;
//         }
//
//         // Invoke the strongly-typed delegate for value types.
//         var typedDelegate = getter.DynamicInvoke(target);
//         return typedDelegate;
//     }
//
//     // Create a delegate for reference type properties: Func<object, object>.
//     private static Func<object, object> CreateReferenceTypeGetterDelegate(PropertyInfo property)
//     {
//         var targetType = property.DeclaringType;
//         var parameter = Expression.Parameter(typeof(object), "target");
//
//         // Cast target (object) to the correct type.
//         var castTarget = Expression.Convert(parameter, targetType);
//
//         // Get property access expression.
//         var propertyAccess = Expression.Property(castTarget, property);
//
//         // Cast property value to object.
//         var castResult = Expression.Convert(propertyAccess, typeof(object));
//
//         // Create lambda expression for Func<object, object>.
//         var lambda = Expression.Lambda<Func<object, object>>(castResult, parameter);
//         return lambda.Compile();
//     }
//
//     // Create a strongly-typed delegate for value type properties (to avoid boxing/unboxing).
//     private static Delegate CreateValueTypeGetterDelegate(PropertyInfo property)
//     {
//         var targetType = property.DeclaringType;
//         var parameter = Expression.Parameter(typeof(object), "target");
//
//         // Cast target (object) to the correct type.
//         var castTarget = Expression.Convert(parameter, targetType);
//
//         // Get property access expression.
//         var propertyAccess = Expression.Property(castTarget, property);
//
//         // Compile a strongly-typed delegate (avoids boxing).
//         var lambda = Expression.Lambda(propertyAccess, parameter);
//         return lambda.Compile();
//     }
// }
