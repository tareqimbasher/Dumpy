using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Dumpy.Html;
using Dumpy.Html.Converters;
using Dumpy.Utils;

// ReSharper disable once CheckNamespace
namespace Dumpy;

public static class HtmlDumper
{
    private static readonly Lazy<HtmlDumpOptions> _defaultOptions = new(() => new HtmlDumpOptions());

    public static string DumpHtml<T>(this T? value, HtmlDumpOptions? options = null)
    {
        var valueType = value?.GetType() ?? typeof(T);
        return DumpHtml(value, valueType, options);
    }

    public static string DumpHtml<T>(this T? value, Type valueType, HtmlDumpOptions? options = null)
    {
        options ??= _defaultOptions.Value;

        var writer = new ValueStringBuilder(stackalloc char[512], 4096);

        try
        {
            DumpHtml(ref writer, value, valueType, options);
        }
        catch
        {
            writer.Dispose();
            throw;
        }

        return writer.ToString();
    }

    public static void DumpHtml<T>(ref ValueStringBuilder writer, T? value, Type valueType, HtmlDumpOptions options)
    {
        var converter = options.GetConverter(valueType);
        converter.ConvertInner(ref writer, value, valueType, options);
    }
}