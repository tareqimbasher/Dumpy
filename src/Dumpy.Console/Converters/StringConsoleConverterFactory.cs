using System;
using Dumpy.Console.Widgets;
using Dumpy.Utils;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class StringConsoleConverterFactory : ConsoleConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return TypeUtil.IsStringFormattable(typeToConvert);
    }

    public override ConsoleConverter? CreateConverter(Type typeToConvert, ConsoleDumpOptions options)
    {
        var converterType = typeof(StringDefaultConsoleConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as ConsoleConverter;
    }
}

public class StringDefaultConsoleConverter<T> : ConsoleConverter<T>
{
    public override IRenderable Convert(T? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.New(options);
        }

        if (targetType == typeof(string))
        {
            return Markup.FromInterpolated(
                options.ShowTextQuotes ? (FormattableString)$"\"{value}\"" : (FormattableString)$"{value}",
                options.Styles.String);
        }

        if (targetType == typeof(bool))
        {
            return Markup.FromInterpolated($"{value}", options.Styles.Boolean);
        }

        if (targetType.IsEnum)
        {
            return new Markup($"{value}", options.Styles.Enum);
        }

        if (targetType == typeof(Guid))
        {
            return Markup.FromInterpolated($"{value}", options.Styles.Guid);
        }

        if (targetType == typeof(DateTime)
#if NET6_0_OR_GREATER
            || targetType == typeof(DateOnly)
            || targetType == typeof(TimeOnly)
#endif
            || targetType == typeof(DateTimeOffset)
            || targetType == typeof(TimeSpan))
        {
            return Markup.FromInterpolated($"{value}", options.Styles.DateAndTime);
        }

        if (targetType == typeof(int)
            || targetType == typeof(uint)
            || targetType == typeof(long)
            || targetType == typeof(ulong)
            || targetType == typeof(short)
            || targetType == typeof(ushort)
            || targetType == typeof(byte)
            || targetType == typeof(sbyte)
            || targetType == typeof(float)
            || targetType == typeof(double)
            || targetType == typeof(decimal))
        {
            return Markup.FromInterpolated($"{value}", options.Styles.Numeric);
        }

        if (targetType == typeof(char))
        {
            return Markup.FromInterpolated(
                options.ShowTextQuotes ? (FormattableString)$"'{value}'" : (FormattableString)$"{value}",
                options.Styles.Char);
        }

        return new Text(value.ToString()!);
    }
}