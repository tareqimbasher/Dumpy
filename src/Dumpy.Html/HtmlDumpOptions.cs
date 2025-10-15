using System.Collections.Generic;
using Dumpy.Html.Converters;

namespace Dumpy.Html;

public partial class HtmlDumpOptions : DumpOptions
{
    /// <summary>
    /// The list of custom HTML converters to use during serialization.
    /// </summary>
    public List<HtmlConverter> Converters { get; set; } = new();

    /// <summary>
    /// If true, adds title attributes containing additional metadata (ex. type name).
    /// </summary>
    public bool AddTitleAttributes { get; set; }

    /// <summary>
    /// CSS classes added to serialized HTML nodes.
    /// </summary>
    public CssClassOptions CssClasses { get; set; } = new();
}

public class CssClassOptions
{
    public const string DefaultNullCssClass = "dm-null";
    public const string DefaultEmptyCollectionCssClass = "dm-empty";
    public const string DefaultCyclicReferenceCssClass = "dm-cyclic";
    public const string DefaultMaxDepthReachedCssClass = "dm-depth-max";
    public const string DefaultTableInfoHeaderCssClass = "dm-t-info";
    public const string DefaultTableDataHeaderCssClass = "dm-t-data";

    private string? _null = DefaultNullCssClass;
    private string? _emptyCollection = DefaultEmptyCollectionCssClass;
    private string? _cyclicReference = DefaultCyclicReferenceCssClass;
    private string? _maxDepthReached = DefaultMaxDepthReachedCssClass;
    private string? _tableInfoHeader = DefaultTableInfoHeaderCssClass;
    private string? _tableDataHeader = DefaultTableDataHeaderCssClass;

    /// <summary>
    /// If true, will add CSS classes to serialized HTML. (Default: true)
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The CSS class added to null values. (Default: <see cref="DefaultNullCssClass"/>)
    /// </summary>
    public string? Null
    {
        get => !Enabled ? null : _null;
        set => _null = value;
    }

    /// <summary>
    /// The CSS class added to empty collections. (Default: <see cref="DefaultEmptyCollectionCssClass"/>)
    /// </summary>
    public string? EmptyCollection
    {
        get => !Enabled ? null : _emptyCollection;
        set => _emptyCollection = value;
    }

    /// <summary>
    /// The CSS class added to cyclic references. (Default: <see cref="DefaultCyclicReferenceCssClass"/>)
    /// </summary>
    public string? CyclicReference
    {
        get => !Enabled ? null : _cyclicReference;
        set => _cyclicReference = value;
    }

    /// <summary>
    /// The CSS class added to max depth reached elements. (Default: <see cref="DefaultMaxDepthReachedCssClass"/>)
    /// </summary>
    public string? MaxDepthReached
    {
        get => !Enabled ? null : _maxDepthReached;
        set => _maxDepthReached = value;
    }

    /// <summary>
    /// The CSS class added to a table's info header. (Default: <see cref="DefaultTableInfoHeaderCssClass"/>)
    /// </summary>
    public string? TableInfoHeader
    {
        get => !Enabled ? null : _tableInfoHeader;
        set => _tableInfoHeader = value;
    }

    /// <summary>
    /// The CSS class added to a table's data header. (Default: <see cref="DefaultTableDataHeaderCssClass"/>)
    /// </summary>
    public string? TableDataHeader
    {
        get => !Enabled ? null : _tableDataHeader;
        set => _tableDataHeader = value;
    }
}