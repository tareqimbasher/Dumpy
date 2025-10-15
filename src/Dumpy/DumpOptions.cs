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
    /// The max number of items to serialize from a collection. Default is int.MaxValue.
    /// </summary>
    public int MaxCollectionItems { get; set; } = int.MaxValue;

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