using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dumpy.Metadata;

public static class BuiltInTypeMetadataProvider
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _typePropertyInfoCache = new();
    private static readonly ConcurrentDictionary<Type, FieldInfo[]> _typeFieldInfoCache = new();

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

    public static PropertyInfo[] GetProperties(Type type, bool includeNonPublic)
    {
        if (_typePropertyInfoCache.TryGetValue(type, out var properties))
        {
            return properties;
        }

        var bindingFlags = includeNonPublic
            ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            : BindingFlags.Instance | BindingFlags.Public;

        properties = type
            .GetProperties(bindingFlags)
            .Where(p => p.CanRead)
            // Exclude properties that exist in base types and are hidden by properties in derived types
            .GroupBy(p => p.Name)
            .Select(g => g.OrderBy(p => p.DeclaringType == type).First())
            .ToArray();

        _typePropertyInfoCache.TryAdd(type, properties);
        return properties;
    }

    public static FieldInfo[] GetFields(Type type, bool includeNonPublic)
    {
        if (_typeFieldInfoCache.TryGetValue(type, out var fields))
        {
            return fields;
        }

        var bindingFlags = includeNonPublic
            ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            : BindingFlags.Instance | BindingFlags.Public;
        
        fields = type.GetFields(bindingFlags);

        _typeFieldInfoCache.TryAdd(type, fields);
        return fields;
    }
}

public class DefaultTypeMetadataProvider : ITypeMetadataProvider
{
    private static readonly ConcurrentDictionary<Type, DataMembers> TypeMemberCache = new();

    public static readonly Lazy<DefaultTypeMetadataProvider> Instance = new(() => new DefaultTypeMetadataProvider());

    public string GetName(Type type, bool fullyQualify = false)
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

    public DataMembers GetDataMembers(Type type)
    {
        if (TypeMemberCache.TryGetValue(type, out var dataMembers))
        {
            return DataMembers.Emtpy;
        }

        var properties = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(p => p.CanRead)
            // Exclude properties that exist in base types and are hidden by properties in derived types
            .GroupBy(p => p.Name)
            .Select(g => g.OrderBy(p => p.DeclaringType == type).First())
            .ToArray();

        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        dataMembers = new DataMembers(properties, fields);

        TypeMemberCache.TryAdd(type, dataMembers);
        return dataMembers;
    }
}