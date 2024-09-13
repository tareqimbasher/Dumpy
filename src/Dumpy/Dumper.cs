using System;
using System.Collections;
using System.Linq;
using Dumpy.Renderers.Html;
using Dumpy.Utils;

namespace Dumpy;

public static class Dumper
{
    private static readonly HtmlDumpOptions DefaultOptions;

    static Dumper()
    {
        DefaultOptions = new();
    }

    public static void ConfigureDefaults(Action<DumpOptions> configure)
    {
        configure(DefaultOptions);
    }

    public static Type? GetUserDefinedConverterType(Type targetType, DumpOptions options)
    {
        if (options.Converters.Count == 0)
        {
            return null;
        }
        
        var target = typeof(IHtmlConverter<>).MakeGenericType(targetType);

        var converterType = options.Converters.FirstOrDefault(x => target.IsAssignableFrom(x));

        return converterType;
    }

    public static IGenericHtmlConverter GetGenericConverter(Type targetType)
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

    public static string DumpHtml<T>(T? value, HtmlDumpOptions? options = null)
    {
        options ??= DefaultOptions;

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
        
        var converter = GetGenericConverter(valueType);
        
        converter.Convert(ref writer, value, valueType, options);
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("o")]
#endif
    public static T? Dump<T>(
        T? o,
        string? title = null,
        int maxDepth = 32,
        int maxCollectionCount = 1000,
        bool usefullyQualifyTypeNames = false,
        bool includeProperties = true,
        bool includeNonPublicProperties = false,
        bool includeFields = false
    )
    {
        return o;
    }

    public static T? Dump<T>(
        T? o,
        string? title = null,
        int maxDepth = 32,
        int maxCollectionCount = 1000
    )
    {
        return o;
    }
}