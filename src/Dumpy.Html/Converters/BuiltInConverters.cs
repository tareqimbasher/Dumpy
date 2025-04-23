using System.Xml;
using System.Xml.Linq;

namespace Dumpy.Html.Converters;

public static class BuiltInConverters
{
    private static HtmlConverter<XmlNode>? _xmlNodeConverter;
    public static HtmlConverter<XmlNode> XmlNodeConverter => _xmlNodeConverter ??= new XmlNodeHtmlConverter();
    
    private static HtmlConverter<XNode>? _xNodeConverter;
    public static HtmlConverter<XNode> XNodeConverter => _xNodeConverter ??= new XNodeHtmlConverter();
}