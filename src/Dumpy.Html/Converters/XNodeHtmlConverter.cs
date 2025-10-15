using System;
using System.Xml.Linq;

namespace Dumpy.Html.Converters;

public class XNodeHtmlConverter : HtmlConverter<XNode>
{
    public override void Convert(ref ValueStringBuilder writer, XNode? value, Type targetType, HtmlDumpOptions options)
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
        writer.Append(value.ToString(SaveOptions.DisableFormatting));
        writer.WriteCloseTag("code");
        writer.WriteCloseTag("pre");
    }
}