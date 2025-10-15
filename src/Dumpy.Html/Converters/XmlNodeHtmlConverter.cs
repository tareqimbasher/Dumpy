using System;
using System.Xml;

namespace Dumpy.Html.Converters;

public class XmlNodeHtmlConverter : HtmlConverter<XmlNode>
{
    public override void Convert(ref ValueStringBuilder writer, XmlNode? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }
        
        writer.WriteOpenTag("pre");
        writer.WriteOpenTagStart("code");
        writer.WriteAttr("language", "xml");
        writer.WriteOpenTagEnd();
        writer.Append(value.OuterXml);
        writer.WriteCloseTag("code");
        writer.WriteCloseTag("pre");
    }
}