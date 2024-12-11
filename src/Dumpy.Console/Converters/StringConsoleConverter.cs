using System;
using Dumpy.Console.Widgets;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class StringConsoleConverter : IGenericConsoleConverter
{
    private static StringConsoleConverter? _instance;
    public static StringConsoleConverter Instance => _instance ??= new StringConsoleConverter();

    public IRenderable Convert<T>(T? value, Type targetType, DumpOptions options)
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