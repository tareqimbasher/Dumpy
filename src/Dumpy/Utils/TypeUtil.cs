using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Dumpy.Utils;

internal static class TypeUtil
{
    public static bool IsStringFormattable(Type type)
    {
        return type.IsPrimitive
               || type == typeof(string)
               || type.IsEnum
               || typeof(IFormattable).IsAssignableFrom(type)
               || typeof(Exception).IsAssignableFrom(type)
               || typeof(Type).IsAssignableFrom(type)
               || Nullable.GetUnderlyingType(type) != null
               || typeof(XNode).IsAssignableFrom(type)
               || typeof(XmlNode).IsAssignableFrom(type)
            ;
    }

    public static bool IsCollection(Type type)
    {
        return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
    }
    
    public static bool IsObject(Type type)
    {
        return !IsStringFormattable(type) && !IsCollection(type);
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
    
    internal static Type? GetCollectionElementType(Type collectionType)
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
