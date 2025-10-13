using System.Data;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#if NETCOREAPP3_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Nodes;
#endif

namespace Dumpy.Html.Converters;

public static class BuiltInConverters
{
#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER
    private static HtmlConverter<ITuple>? _tupleConverter;
    public static HtmlConverter<ITuple> TupleConverter => _tupleConverter ??= new TupleHtmlConverter();
#endif

    private static HtmlConverter<DataTable>? _dataTableNodeConverter;

    public static HtmlConverter<DataTable> DataTableNodeConverter =>
        _dataTableNodeConverter ??= new DataTableHtmlConverter();

    private static HtmlConverter<XmlNode>? _xmlNodeConverter;
    public static HtmlConverter<XmlNode> XmlNodeConverter => _xmlNodeConverter ??= new XmlNodeHtmlConverter();

    private static HtmlConverter<XNode>? _xNodeConverter;
    public static HtmlConverter<XNode> XNodeConverter => _xNodeConverter ??= new XNodeHtmlConverter();

#if NETCOREAPP3_0_OR_GREATER
    private static HtmlConverter<JsonDocument>? _jsonDocumentConverter;

    public static HtmlConverter<JsonDocument> JsonDocumentConverter =>
        _jsonDocumentConverter ??= new JsonDocumentHtmlConverter();

    private static HtmlConverter<JsonElement>? _jsonElementConverter;

    public static HtmlConverter<JsonElement> JsonElementConverter =>
        _jsonElementConverter ??= new JsonElementHtmlConverter();

    private static HtmlConverter<JsonNode>? _jsonNodeConverter;
    public static HtmlConverter<JsonNode> JsonNodeConverter => _jsonNodeConverter ??= new JsonNodeHtmlConverter();
#endif
}