using System;
using System.Collections;
using System.Linq;
using Dumpy.Renderers.Html;
using Dumpy.Utils;

// ReSharper disable once CheckNamespace
namespace Dumpy;

public static partial class Dumper
{
    private static readonly Lazy<HtmlDumpOptions> DefaultHtmlOptions = new(() => new HtmlDumpOptions());

    public static string DumpHtml<T>(T? value, HtmlDumpOptions? options = null)
    {
        options ??= DefaultHtmlOptions.Value;

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
        var userDefinedConverterType = GetUserDefinedHtmlConverterType(valueType, options);

        if (userDefinedConverterType != null)
        {
            var userDefinedConverter = Activator.CreateInstance(userDefinedConverterType) as IHtmlConverter<T>
                                       ?? throw new InvalidOperationException(
                                           $"Cannot create instance of {userDefinedConverterType} as {nameof(IHtmlConverter<T>)}.");

            userDefinedConverter.Convert(ref writer, value, valueType, options);
            return;
        }
        
        var converter = GetGenericHtmlConverter(valueType);
        
        converter.Convert(ref writer, value, valueType, options);
    }
    
    internal static Type? GetUserDefinedHtmlConverterType(Type targetType, DumpOptions options)
    {
        if (options.Converters.Count == 0)
        {
            return null;
        }
        
        var target = typeof(IHtmlConverter<>).MakeGenericType(targetType);

        var converterType = options.Converters.FirstOrDefault(x => target.IsAssignableFrom(x));

        return converterType;
    }

    internal static IGenericHtmlConverter GetGenericHtmlConverter(Type targetType)
    {
        if (TypeUtil.IsStringFormattable(targetType))
        {
            return StringFormattableHtmlConverter.Instance;
        }

        if (typeof(IEnumerable).IsAssignableFrom(targetType))
        {
            return CollectionHtmlConverter.Instance;
        }

        return ObjectHtmlConverter.Instance;
    }
}