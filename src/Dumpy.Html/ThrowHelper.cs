using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Dumpy.Html;

public static class ThrowHelper
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNotSupportedException_BuiltInConvertersNotRooted(Type typeToConvert)
    {
        throw new NotSupportedException($"Built-in converters are not rooted. Attempting to convert: {typeToConvert}");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidOperationException_ConverterFactoryReturnsNullConverter(Type converterFactoryType,
        Type typeToConvert)
    {
        throw new InvalidOperationException(
            $"Converter factory '{converterFactoryType}' returned null when asked to create a converter for type '{typeToConvert}'");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidOperationException_ConverterFactoryReturnsConverterFactory(
        Type converterFactoryType,
        Type typeToConvert)
    {
        throw new InvalidOperationException(
            $"Converter factory '{converterFactoryType}' returned another factory when asked to create a converter for type '{typeToConvert}'");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidOperationException_ConverterNotCompatible(Type converterType, Type typeToConvert)
    {
        throw new InvalidOperationException(
            $"Converter '{converterType}' does not support converting the type '{typeToConvert}'");
    }
}