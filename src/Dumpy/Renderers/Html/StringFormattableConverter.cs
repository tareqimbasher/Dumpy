using System;
using Dumpy.Renderers.Html.Utils;

namespace Dumpy.Renderers.Html;

public class StringFormattableConverter : IGenericConverter
{
    private static StringFormattableConverter? _instance;

    public static StringFormattableConverter Instance
    {
        get
        {
            return _instance ??= new StringFormattableConverter();
        }
    }
    
    public void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, DumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNull(options);
            return;
        }
        
        writer.Append(HtmlUtil.EscapeText(value.ToString()));
    }
}