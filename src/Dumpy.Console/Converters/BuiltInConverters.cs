using System.Xml;
using System.Xml.Linq;

namespace Dumpy.Console.Converters;

public static class BuiltInConverters
{
    private static ConsoleConverter<XmlNode>? _xmlNodeConverter;
    public static ConsoleConverter<XmlNode> XmlNodeConverter => _xmlNodeConverter ??= new XmlNodeConsoleConverter();

    private static ConsoleConverter<XNode>? _xNodeConverter;
    public static ConsoleConverter<XNode> XNodeConverter => _xNodeConverter ??= new XNodeConsoleConverter();
}