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
    private static readonly Lazy<HtmlDumpOptions> DefaultOptions = new(() => new HtmlDumpOptions());

    public static string DumpHtml<T>(this T? value, HtmlDumpOptions? options = null)
    {
        var valueType = value?.GetType() ?? typeof(T);
        return DumpHtml(value, valueType, options);
    }

    public static string DumpHtml<T>(this T? value, Type valueType, HtmlDumpOptions? options = null)
    {
        options ??= DefaultOptions.Value;

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
        
        // The problem right now is how to call the Convert method

        // var method = typeof(HtmlConverter<>).GetMethod(
        //     "Convert", 
        //     BindingFlags.Public | BindingFlags.Instance);
        // Debug.Assert(method != null);
        //
        // method.Invoke(converter, new object[] { writer, value!, valueType, options });
    }

    // internal static Type? GetUserDefinedConverterType(Type targetType, DumpOptions options)
    // {
    //     if (options.Converters.Count == 0)
    //     {
    //         return null;
    //     }
    //
    //     var target = typeof(IHtmlConverter<>).MakeGenericType(targetType);
    //
    //     var converterType = options.Converters.FirstOrDefault(x => target.IsAssignableFrom(x));
    //
    //     return converterType;
    // }
    //
    // internal static IGenericHtmlConverter GetGenericConverter(Type targetType)
    // {
    //     if (TypeUtil.IsStringFormattable(targetType))
    //     {
    //         return StringHtmlConverterFactory.Instance;
    //     }
    //
    //     if (TypeUtil.IsCollection(targetType))
    //     {
    //         return IEnumerableHtmlConverterFactory.Instance;
    //     }
    //
    //     return ObjectHtmlConverterFactory.Instance;
    // }
}