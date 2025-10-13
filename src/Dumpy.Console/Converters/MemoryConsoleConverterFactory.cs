using System;
using System.Collections.Generic;
using System.Reflection;
using Dumpy.Console.Widgets;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class MemoryConsoleConverterFactory : ConsoleConverterFactory
{
    private static readonly HashSet<Type> _canConvertTypes = new()
    {
        typeof(Memory<>),
        typeof(ReadOnlyMemory<>),
    };
    
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && _canConvertTypes.Contains(typeToConvert.GetGenericTypeDefinition());
    }
    
    public override ConsoleConverter? CreateConverter(Type typeToConvert, ConsoleDumpOptions options)
    {
        var converterType = typeof(MemoryConsoleConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as ConsoleConverter;
    }
}

public class MemoryConsoleConverter<T> : ConsoleConverter<T>
{
    private const string ToArrayMethodName = "ToArray";
    
    public override IRenderable Convert(T? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.Instance;
        }
        
        var method = targetType.GetMethod(ToArrayMethodName, BindingFlags.Public | BindingFlags.Instance);

        if (method == null)
        {
            throw new SerializationException(
                $"Cannot serialize type {targetType}. {ToArrayMethodName} method not found.");
        }

        var array = (Array)method.Invoke(value, [])!;

        return ConsoleDumper.DumpToRenderable(array, array.GetType(), options);
    }
}