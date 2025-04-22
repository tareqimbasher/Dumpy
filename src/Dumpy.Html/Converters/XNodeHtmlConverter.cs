using System;
using System.Xml.Linq;
using Dumpy.Html.Utils;

namespace Dumpy.Html.Converters;

public class XNodeHtmlConverter : IHtmlConverter<XNode>
{
    public void Convert(ref ValueStringBuilder writer, XNode? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }
        
        writer.WriteOpenTag("pre");
        writer.WriteOpenTag("code", "language=\"xml\"");
        
        writer.Append(HtmlUtil.EscapeText(value.ToString(SaveOptions.DisableFormatting)));
        
        writer.WriteCloseTag("code");
        writer.WriteCloseTag("pre");
    }
}