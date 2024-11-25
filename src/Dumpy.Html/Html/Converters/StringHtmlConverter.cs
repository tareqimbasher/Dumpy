using System;
using Dumpy.Html.Utils;

namespace Dumpy.Html.Converters;

public class StringHtmlConverter : IGenericHtmlConverter
{
    private static StringHtmlConverter? _instance;
    public static StringHtmlConverter Instance => _instance ??= new StringHtmlConverter();
    
    public void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }
        
        writer.Append(HtmlUtil.EscapeText(value.ToString()));
    }
}