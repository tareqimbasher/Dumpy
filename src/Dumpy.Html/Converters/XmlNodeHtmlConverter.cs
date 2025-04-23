using System;
using System.Xml;
using Dumpy.Html.Utils;

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
        writer.WriteOpenTag("code", "language=\"xml\"");
        
        writer.Append(HtmlUtil.EscapeText(value.OuterXml));
        
        writer.WriteCloseTag("code");
        writer.WriteCloseTag("pre");
    }
}