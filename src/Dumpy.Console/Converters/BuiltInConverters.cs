using System.Data;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#if NETCOREAPP3_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Nodes;
#endif

namespace Dumpy.Console.Converters;

internal static class BuiltInConverters
{
#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER
    private static ConsoleConverter<ITuple>? _tupleConverter;
    public static ConsoleConverter<ITuple> TupleConverter => _tupleConverter ??= new TupleConsoleConverter();
#endif

    private static ConsoleConverter<DataTable>? _dataTableNodeConverter;

    public static ConsoleConverter<DataTable> DataTableNodeConverter =>
        _dataTableNodeConverter ??= new DataTableConsoleConverter();
    
    private static ConsoleConverter<DataSet>? _dataSetNodeConverter;

    public static ConsoleConverter<DataSet> DataSetNodeConverter =>
        _dataSetNodeConverter ??= new DataSetConsoleConverter();

    private static ConsoleConverter<XmlNode>? _xmlNodeConverter;
    public static ConsoleConverter<XmlNode> XmlNodeConverter => _xmlNodeConverter ??= new XmlNodeConsoleConverter();

    private static ConsoleConverter<XNode>? _xNodeConverter;
    public static ConsoleConverter<XNode> XNodeConverter => _xNodeConverter ??= new XNodeConsoleConverter();

#if NETCOREAPP3_0_OR_GREATER
    private static ConsoleConverter<JsonDocument>? _jsonDocumentConverter;

    public static ConsoleConverter<JsonDocument> JsonDocumentConverter =>
        _jsonDocumentConverter ??= new JsonDocumentConsoleConverter();

    private static ConsoleConverter<JsonElement>? _jsonElementConverter;

    public static ConsoleConverter<JsonElement> JsonElementConverter =>
        _jsonElementConverter ??= new JsonElementConsoleConverter();

    private static ConsoleConverter<JsonNode>? _jsonNodeConverter;
    public static ConsoleConverter<JsonNode> JsonNodeConverter => _jsonNodeConverter ??= new JsonNodeConsoleConverter();
#endif
}