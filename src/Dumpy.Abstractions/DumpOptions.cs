using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Dumpy.Utils;

namespace Dumpy;

/// <summary>
/// Provides options to use with a dumper.
/// </summary>
public class DumpOptions
{
    private int _maxDepth = 10;
    private int _maxCollectionItems = int.MaxValue;
    private readonly ConcurrentDictionary<Type, MemberInfo[]> _readableMembersCache = new();

    /// <summary>
    /// How reference loops should be handled. (Default: Error)
    /// </summary>
    public ReferenceLoopHandling ReferenceLoopHandling { get; set; } = ReferenceLoopHandling.Error;

    /// <summary>
    /// The max serialization depth. Defaults to 10.
    /// </summary>
    public int MaxDepth
    {
        get => _maxDepth;
        set
        {
            if (value < 0)
            {
                throw new InvalidOperationException("Max depth cannot be negative.");
            }

            _maxDepth = value;
        }
    }

    /// <summary>
    /// The max number of items to include from a collection. Defaults to int.MaxValue.
    /// </summary>
    public int MaxCollectionItems
    {
        get => _maxCollectionItems;
        set
        {
            if (value < 0)
            {
                throw new InvalidOperationException("Max collection items cannot be negative.");
            }

            _maxCollectionItems = value;
        }
    }

    /// <summary>
    /// If true, will include public fields in the output. Defaults to false.
    /// </summary>
    public bool IncludeFields { get; set; }

    /// <summary>
    /// If true, will include non-public fields and properties in the output. Defaults to false.
    /// </summary>
    public bool IncludeNonPublicMembers { get; set; }

    /// <summary>
    /// A predicate to filter members that should be included in the output.
    /// </summary>
    public Func<MemberInfo, bool>? MemberFilter { get; set; }

    /// <summary>
    /// Gets all members that can be read from the specified target type based on the rules
    /// defined in this options instance.
    /// </summary>
    /// <param name="targetType">The type to inspect.</param>
    public MemberInfo[] GetReadableMembers(Type targetType)
    {
        if (_readableMembersCache.TryGetValue(targetType, out var cached))
        {
            return cached;
        }

        var result = BuildReadableMembers(targetType);
        _readableMembersCache.TryAdd(targetType, result);
        return result;
    }

    private MemberInfo[] BuildReadableMembers(Type targetType)
    {
        var properties = TypeUtil.GetReadableProperties(targetType, IncludeNonPublicMembers);
        var fields = IncludeFields ? TypeUtil.GetFields(targetType, IncludeNonPublicMembers) : null;

        if (MemberFilter == null)
        {
            if (fields == null || fields.Length == 0)
            {
                return properties;
            }

            var members = new MemberInfo[properties.Length + fields.Length];
            properties.CopyTo(members, 0);
            fields.CopyTo(members, properties.Length);
            return members;
        }

        // With a filter we must evaluate each member
        var filtered = new List<MemberInfo>(properties.Length + (fields?.Length ?? 0));
        foreach (var prop in properties)
        {
            if (MemberFilter(prop))
            {
                filtered.Add(prop);
            }
        }

        if (fields != null)
        {
            foreach (var field in fields)
            {
                if (MemberFilter(field))
                {
                    filtered.Add(field);
                }
            }
        }

        return filtered.ToArray();
    }
}