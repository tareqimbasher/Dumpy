using System;
using System.Collections.Generic;
using Dumpy.Metadata;

namespace Dumpy;

public class DumpOptions
{
    public const ReferenceLoopHandling DefaultReferenceLoopHandling = ReferenceLoopHandling.Error;
    public const int DefaultMaxDepth = 64;

    private int _maxDepth = DefaultMaxDepth;
    private ITypeMetadataProvider? _typeMetadataProvider;

    public ITypeMetadataProvider TypeMetadataProvider
    {
        get => _typeMetadataProvider ??= DefaultTypeMetadataProvider.Instance.Value;
        set => _typeMetadataProvider = value;
    }

    /// <summary>
    /// How reference loops should be handled. (Default: Error)
    /// </summary>
    public ReferenceLoopHandling ReferenceLoopHandling { get; set; } = DefaultReferenceLoopHandling;

    /// <summary>
    /// If true, empty collections, that are not the root object being serialized, will not be serialized. (Default: false)
    /// </summary>
    public bool DoNotSerializeNonRootEmptyCollections { get; set; }

    /// <summary>
    /// If true, adds title attributes containing additional metadata.
    /// </summary>
    public bool AddTitleAttributes { get; set; }
    
    /// <summary>
    /// If true, will serialize fields.
    /// </summary>
    public bool IncludeFields { get; set; }
    
    /// <summary>
    /// If true, will serialize non-public fields and properties.
    /// </summary>
    public bool IncludeNonPublicMembers { get; set; }

    /// <summary>
    /// If set, only this number of items will be serialized when serializing collections. (Default: null)
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

    /// <summary>
    /// The list of custom HTML Converters to use during serialization.
    /// </summary>
    public List<Type> Converters { get; set; } = new();

    /// <summary>
    /// CSS classes added to serialized HTML nodes.
    /// </summary>
    public CssClasses CssClasses { get; } = new();
}

public class CssClasses
{
    public const string DefaultNullCssClass = "dm-null";
    public const string DefaultEmptyCollectionCssClass = "dm-empty";
    public const string DefaultCyclicReferenceCssClass = "dm-cyclic";
    public const string DefaultMaxDepthReachedCssClass = "dm-depth-exceeded";
    public const string DefaultTableInfoHeaderCssClass = "dm-t-info";
    public const string DefaultTableDataHeaderCssClass = "dm-t-data";

    /// <summary>
    /// The CSS class added to null values. (Default: <see cref="DefaultNullCssClass"/>)
    /// </summary>
    public string Null { get; set; } = DefaultNullCssClass;

    /// <summary>
    /// The CSS class added to empty collections. (Default: <see cref="DefaultEmptyCollectionCssClass"/>)
    /// </summary>
    public string EmptyCollection { get; set; } = DefaultEmptyCollectionCssClass;

    /// <summary>
    /// The CSS class added to cyclic references. (Default: <see cref="DefaultCyclicReferenceCssClass"/>)
    /// </summary>
    public string CyclicReference { get; set; } = DefaultCyclicReferenceCssClass;

    /// <summary>
    /// The CSS class added to max depth reached elements. (Default: <see cref="DefaultMaxDepthReachedCssClass"/>)
    /// </summary>
    public string MaxDepthReached { get; set; } = DefaultMaxDepthReachedCssClass;

    /// <summary>
    /// The CSS class added to a table's info header. (Default: <see cref="DefaultTableInfoHeaderCssClass"/>)
    /// </summary>
    public string TableInfoHeader { get; set; } = DefaultTableInfoHeaderCssClass;

    /// <summary>
    /// The CSS class added to a table's data header. (Default: <see cref="DefaultTableDataHeaderCssClass"/>)
    /// </summary>
    public string TableDataHeader { get; set; } = DefaultTableDataHeaderCssClass;
}