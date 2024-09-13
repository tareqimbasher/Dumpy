using System;

namespace Dumpy.Renderers.Html;

public interface IGenericHtmlConverter
{
    void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options);
}

public interface IHtmlConverter<in T>
{
    void Convert(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options);
}
