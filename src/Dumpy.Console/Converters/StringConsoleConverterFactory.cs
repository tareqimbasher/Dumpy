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
            return NullWidget.Instance;
        }

        if (targetType == typeof(string))
        {
            return new Markup($"[lightsalmon1]\"{value}\"[/]");
        }
        
        if (targetType == typeof(char))
        {
            return new Markup($"[lightsalmon1]'{value}'[/]");
        }
        
        if (targetType == typeof(Guid))
        {
            return new Markup($"[lightsalmon1]{value}[/]");
        }

        if (targetType.IsEnum
            || targetType == typeof(DateTime) 
            || targetType == typeof(DateOnly) 
            || targetType == typeof(DateTimeOffset) 
            || targetType == typeof(TimeSpan))
        {
            return new Markup($"[yellow4_1]{value}[/]");
        }
        
        if (targetType == typeof(bool))
        {
            return new Markup($"[mediumpurple1]{value}[/]");
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
            return new Markup($"[skyblue2]{value}[/]");
        }

        return new Text(value.ToString()!);
    }
}