using System;
using System.Collections;
using System.Linq;
using Dumpy.Metadata;
using Dumpy.Renderers;
using Dumpy.Renderers.Html;
using Dumpy.Utils;

namespace Dumpy;

public static class Dumper
{
    private static readonly DumpOptions DefaultOptions;

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
        
        var target = typeof(IConverter<>).MakeGenericType(targetType);

        var converterType = options.Converters.FirstOrDefault(x => target.IsAssignableFrom(x));

        return converterType;
    }

    public static IGenericConverter GetGenericConverter(Type targetType)
    {
        if (TypeUtil.IsStringFormattable(targetType))
        {
            return StringFormattableConverter.Instance;
        }

        if (typeof(IEnumerable).IsAssignableFrom(targetType))
        {
            return CollectionConverter.Instance;
        }

        return ObjectConverter.Instance;
    }

    public static string DumpHtml<T>(T? value, DumpOptions? options = null)
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

    public static void DumpHtml<T>(ref ValueStringBuilder writer, T? value, Type valueType, DumpOptions options)
    {
        var userDefinedConverterType = GetUserDefinedConverterType(valueType, options);

        if (userDefinedConverterType != null)
        {
            var userDefinedConverter = Activator.CreateInstance(userDefinedConverterType) as IConverter<T>
                            ?? throw new InvalidOperationException(
                                $"Cannot create instance of {userDefinedConverterType} as {nameof(IConverter<T>)}.");

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
        int maxCollectionCount = 1000,
        ITypeMetadataProvider? typeMetadataProvider = null
    )
    {
        return o;
    }
}