using System;

namespace Dumpy.Html.Converters;

public interface IHtmlConverter<in T>
{
    void Convert(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options);
}