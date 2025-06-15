using System;
using System.Reflection;

namespace Dumpy;

public class DumpOptions
{
    public const ReferenceLoopHandling DefaultReferenceLoopHandling = ReferenceLoopHandling.Error;
    public const int DefaultMaxDepth = 64;

    private int _maxDepth = DefaultMaxDepth;

    /// <summary>
    /// How reference loops should be handled. (Default: Error)
    /// </summary>
    public ReferenceLoopHandling ReferenceLoopHandling { get; set; } = DefaultReferenceLoopHandling;

    /// <summary>
    /// If true, empty collections will be serialized to an "empty" placeholder instead of a table.
    /// </summary>
    public bool SerializeEmptyCollectionPlaceholder { get; set; }

    /// <summary>
    /// If true, will serialize fields.
    /// </summary>
    public bool IncludeFields { get; set; }

    /// <summary>
    /// If true, will serialize non-public fields and properties.
    /// </summary>
    public bool IncludeNonPublicMembers { get; set; }

    /// <summary>
    /// A predicate to filter members that should be dumped.
    /// </summary>
    public Func<MemberInfo, bool>? MemberFilter { get; set; }
    
    /// <summary>
    /// If set, only this number of items will be serialized when serializing collections.
    /// </summary>
    public int? MaxCollectionSerializeLength { get; set; }

    /// <summary>
    /// The max serialization depth. (Default: 64)
    /// </summary>
    public int MaxDepth
    {
        get => _maxDepth;
        set
        {
            if (value < 0)
            {
                _maxDepth = 0;
            }

            _maxDepth = value;
        }
    }
}