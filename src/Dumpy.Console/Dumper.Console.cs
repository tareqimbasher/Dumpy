using System;
using System.Collections;
using System.Linq;
using Dumpy.Console.Converters;
using Dumpy.Utils;
using Spectre.Console.Rendering;

// ReSharper disable once CheckNamespace
namespace Dumpy;

public static class Dumper
{
    private static readonly Lazy<DumpOptions> DefaultOptions = new(() => new DumpOptions());

    public static IRenderable DumpConsole<T>(this T? value, DumpOptions? options = null)
    {
        options ??= DefaultOptions.Value;

        var valueType = value?.GetType() ?? typeof(T);

        return DumpConsole(value, valueType, options);
    }

    public static IRenderable DumpConsole<T>(this T? value, Type valueType, DumpOptions options)
    {
        var userDefinedConverterType = GetUserDefinedConverterType(valueType, options);

        if (userDefinedConverterType != null)
        {
            var userDefinedConverter = Activator.CreateInstance(userDefinedConverterType) as IConsoleConverter<T>
                                       ?? throw new InvalidOperationException(
                                           $"Cannot create instance of {userDefinedConverterType} as {nameof(IConsoleConverter<T>)}.");

            return userDefinedConverter.Convert(value, valueType, options);
        }

        IGenericConsoleConverter converter = GetGenericConverter(valueType);
        return converter.Convert(value, valueType, options);
    }

    internal static Type? GetUserDefinedConverterType(Type targetType, DumpOptions options)
    {
        if (options.Converters.Count == 0)
        {
            return null;
        }

        var target = typeof(IConsoleConverter<>).MakeGenericType(targetType);

        var converterType = options.Converters.FirstOrDefault(x => target.IsAssignableFrom(x));

        return converterType;
    }


    internal static IGenericConsoleConverter GetGenericConverter(Type targetType)
    {
        if (TypeUtil.IsStringFormattable(targetType))
        {
            return StringConsoleConverter.Instance;
        }
        
        if (typeof(IEnumerable).IsAssignableFrom(targetType))
        {
            return CollectionConsoleConverter.Instance;
        }

        return ObjectConsoleConverter.Instance;
    }
}