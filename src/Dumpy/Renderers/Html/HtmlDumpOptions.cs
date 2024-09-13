namespace Dumpy.Renderers.Html;

public class HtmlDumpOptions : DumpOptions
{
    /// <summary>
    /// If true, adds title attributes containing additional metadata.
    /// </summary>
    public bool AddTitleAttributes { get; set; }
    
    /// <summary>
    /// CSS classes added to serialized HTML nodes.
    /// </summary>
    public CssClassOptions CssClasses { get; } = new();
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
    private string? _nullFormatted = $"class=\"{DefaultNullCssClass}\"";
    private string? _emptyCollection = DefaultEmptyCollectionCssClass;
    private string? _emptyCollectionFormatted = $"class=\"{DefaultEmptyCollectionCssClass}\"";
    private string? _cyclicReference = DefaultCyclicReferenceCssClass;
    private string? _cyclicReferenceFormatted = $"class=\"{DefaultCyclicReferenceCssClass}\"";
    private string? _maxDepthReached = DefaultMaxDepthReachedCssClass;
    private string? _maxDepthReachedFormatted = $"class=\"{DefaultMaxDepthReachedCssClass}\"";
    private string? _tableInfoHeader = DefaultTableInfoHeaderCssClass;
    private string? _tableInfoHeaderFormatted = $"class=\"{DefaultTableInfoHeaderCssClass}\"";
    private string? _tableDataHeader = DefaultTableDataHeaderCssClass;
    private string? _tableDataHeaderFormatted = $"class=\"{DefaultTableDataHeaderCssClass}\"";

    /// <summary>
    /// If true, will add CSS classes to serialized HTML. (Default: true)
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The CSS class added to null values. (Default: <see cref="DefaultNullCssClass"/>)
    /// </summary>
    public string? Null
    {
        get => _null;
        set
        {
            _null = value;
            _nullFormatted = $"class=\"{value}\"";
        }
    }
    
    internal string? NullFormatted => Enabled ? _nullFormatted : null;

    /// <summary>
    /// The CSS class added to empty collections. (Default: <see cref="DefaultEmptyCollectionCssClass"/>)
    /// </summary>
    public string? EmptyCollection
    {
        get => _emptyCollection;
        set
        {
            _emptyCollection = value;
            _emptyCollectionFormatted = $"class=\"{value}\"";
        }
    }
    
    internal string? EmptyCollectionFormatted => Enabled ? _emptyCollectionFormatted : null;

    /// <summary>
    /// The CSS class added to cyclic references. (Default: <see cref="DefaultCyclicReferenceCssClass"/>)
    /// </summary>
    public string? CyclicReference
    {
        get => _cyclicReference;
        set
        {
            _cyclicReference = value;
            _cyclicReferenceFormatted = $"class=\"{value}\"";
        }
    }
    
    internal string? CyclicReferenceFormatted => Enabled ? _cyclicReferenceFormatted : null;

    /// <summary>
    /// The CSS class added to max depth reached elements. (Default: <see cref="DefaultMaxDepthReachedCssClass"/>)
    /// </summary>
    public string? MaxDepthReached
    {
        get => _maxDepthReached;
        set
        {
            _maxDepthReached = value;
            _maxDepthReachedFormatted = $"class=\"{value}\"";
        }
    }
    
    internal string? MaxDepthReachedFormatted => Enabled ? _maxDepthReachedFormatted : null;

    /// <summary>
    /// The CSS class added to a table's info header. (Default: <see cref="DefaultTableInfoHeaderCssClass"/>)
    /// </summary>
    public string? TableInfoHeader
    {
        get => _tableInfoHeader;
        set
        {
            _tableInfoHeader = value;
            _tableInfoHeaderFormatted = $"class=\"{value}\"";
        }
    }
    
    internal string? TableInfoHeaderFormatted => Enabled ? _tableInfoHeaderFormatted : null;

    /// <summary>
    /// The CSS class added to a table's data header. (Default: <see cref="DefaultTableDataHeaderCssClass"/>)
    /// </summary>
    public string? TableDataHeader
    {
        get => _tableDataHeader;
        set
        {
            _tableDataHeader = value;
            _tableDataHeaderFormatted = $"class=\"{value}\"";
        }
    }
    
    internal string? TableDataHeaderFormatted => Enabled ? _tableDataHeaderFormatted : null;
}