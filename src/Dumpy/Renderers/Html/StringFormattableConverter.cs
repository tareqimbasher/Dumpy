using System;
using Dumpy.Renderers.Html.Utils;

namespace Dumpy.Renderers.Html;

public class StringFormattableHtmlConverter : IGenericHtmlConverter
{
    private static StringFormattableHtmlConverter? _instance;

    public static StringFormattableHtmlConverter Instance
    {
        get
        {
            return _instance ??= new StringFormattableHtmlConverter();
        }
    }
    
    public void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNull(options);
            return;
        }
        
        writer.Append(HtmlUtil.EscapeText(value.ToString()));
    }
}