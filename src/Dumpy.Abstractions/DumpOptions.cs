using System;
using System.Collections.Generic;
using System.Linq;
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
        var members = new List<MemberInfo>();

        // Properties first, then fields
        members.AddRange(TypeUtil.GetReadableProperties(targetType, IncludeNonPublicMembers));

        if (IncludeFields)
        {
            members.AddRange(TypeUtil.GetFields(targetType, IncludeNonPublicMembers));
        }

        if (MemberFilter != null)
        {
            return members.Where(MemberFilter).ToArray();
        }

        return members.ToArray();
    }
}