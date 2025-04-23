using System;
using Dumpy.Html.Utils;
using Dumpy.Utils;

namespace Dumpy.Html.Converters;

public class StringHtmlConverterFactory : HtmlConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return TypeUtil.IsStringFormattable(typeToConvert);
    }
    
    public override HtmlConverter? CreateConverter(Type typeToConvert, HtmlDumpOptions options)
    {
        var converterType = typeof(StringDefaultHtmlConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as HtmlConverter;
    }
}

public class StringDefaultHtmlConverter<T> : HtmlConverter<T>
{
    public override void Convert(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }
        
        writer.Append(HtmlUtil.EscapeText(value.ToString()));
    }
}