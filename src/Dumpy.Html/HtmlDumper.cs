using System;
using System.Linq;
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
        options ??= DefaultOptions.Value;

        var writer = new ValueStringBuilder(stackalloc char[512], 4096);

        try
        {
            var valueType = value?.GetType() ?? typeof(T);

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
        var userDefinedConverterType = GetUserDefinedConverterType(valueType, options);

        if (userDefinedConverterType != null)
        {
            var userDefinedConverter = Activator.CreateInstance(userDefinedConverterType) as IHtmlConverter<T>
                                       ?? throw new InvalidOperationException(
                                           $"Cannot create instance of {userDefinedConverterType} as {nameof(IHtmlConverter<T>)}.");

            userDefinedConverter.Convert(ref writer, value, valueType, options);
            return;
        }

        IGenericHtmlConverter converter = GetGenericConverter(valueType);
        converter.Convert(ref writer, value, valueType, options);
    }

    internal static Type? GetUserDefinedConverterType(Type targetType, DumpOptions options)
    {
        if (options.Converters.Count == 0)
        {
            return null;
        }

        var target = typeof(IHtmlConverter<>).MakeGenericType(targetType);

        var converterType = options.Converters.FirstOrDefault(x => target.IsAssignableFrom(x));

        return converterType;
    }

    internal static IGenericHtmlConverter GetGenericConverter(Type targetType)
    {
        if (TypeUtil.IsStringFormattable(targetType))
        {
            return StringHtmlConverter.Instance;
        }

        if (TypeUtil.IsCollection(targetType))
        {
            return CollectionHtmlConverter.Instance;
        }

        return ObjectHtmlConverter.Instance;
    }
}