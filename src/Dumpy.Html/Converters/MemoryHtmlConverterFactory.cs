using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dumpy.Html.Converters;

public class MemoryHtmlConverterFactory : HtmlConverterFactory
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

    public override HtmlConverter? CreateConverter(Type typeToConvert, HtmlDumpOptions options)
    {
        var converterType = typeof(MemoryHtmlConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as HtmlConverter;
    }
}

public class MemoryHtmlConverter<T> : HtmlConverter<T>
{
    private const string ToArrayMethodName = "ToArray";

    public override void Convert(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options)
    {
        var method = targetType.GetMethod(ToArrayMethodName, BindingFlags.Public | BindingFlags.Instance);

        if (method == null)
        {
            throw new SerializationException(
                $"Cannot serialize type {targetType}. {ToArrayMethodName} method not found.");
        }

        var array = (Array)method.Invoke(value, [])!;

        HtmlDumper.DumpHtml(ref writer, array, array.GetType(), options);
    }
}