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
        
        return new Markup($"{value}");
    }
}