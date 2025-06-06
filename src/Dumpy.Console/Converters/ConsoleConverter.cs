using System;
using System.Diagnostics;
using Dumpy.Utils;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public abstract class ConsoleConverter
{
    /// <summary>
    /// Determines whether the type can be converted.
    /// </summary>
    /// <param name="typeToConvert">The type is checked as to whether it can be converted.</param>
    /// <returns>True if the type can be converted, false otherwise.</returns>
    public abstract bool CanConvert(Type typeToConvert);
    
    /// <summary>
    /// This is used internally to quickly determine the type being converted for ConsoleConverter of T.
    /// </summary>
    internal abstract Type? TypeToConvert { get; }
    
    public abstract IRenderable ConvertInner(object? value, Type targetType, ConsoleDumpOptions options);
}

public abstract class ConsoleConverter<T> : ConsoleConverter
{
    /// <summary>
    /// Determines whether the type can be converted.
    /// </summary>
    /// <remarks>
    /// The default implementation is to return True when <paramref name="typeToConvert"/> equals typeof(T).
    /// </remarks>
    /// <returns>True if the type can be converted, False otherwise.</returns>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(T) || typeof(T).IsAssignableFromInternal(typeToConvert);
    }
    
    internal sealed override Type TypeToConvert => typeof(T);
    
    public abstract IRenderable Convert(T? value, Type targetType, ConsoleDumpOptions options);
    
    public override IRenderable ConvertInner(object? value, Type targetType, ConsoleDumpOptions options)
    {
        return Convert((T?)value, targetType, options);
    }
}

public abstract class ConsoleConverterFactory : ConsoleConverter
{
    internal sealed override Type? TypeToConvert => null;
    
    /// <summary>
    /// Create a converter for the provided <see cref="Type"/>.
    /// </summary>
    /// <param name="typeToConvert">The <see cref="Type"/> being converted.</param>
    /// <param name="options">The <see cref="ConsoleDumpOptions"/> being used.</param>
    /// <returns>
    /// An instance of a <see cref="ConsoleConverter{T}"/> where T is compatible with <paramref name="typeToConvert"/>.
    /// If <see langword="null"/> is returned, a <see cref="NotSupportedException"/> will be thrown.
    /// </returns>
    public abstract ConsoleConverter? CreateConverter(Type typeToConvert, ConsoleDumpOptions options);
    
    internal ConsoleConverter GetConverterInternal(Type typeToConvert, ConsoleDumpOptions options)
    {
        Debug.Assert(CanConvert(typeToConvert));

        ConsoleConverter? converter = CreateConverter(typeToConvert, options);
        if (converter == null)
        {
            ThrowHelper.ThrowInvalidOperationException_ConverterFactoryReturnsNullConverter(GetType(), typeToConvert);
        }

        if (converter is ConsoleConverterFactory)
        {
            ThrowHelper.ThrowInvalidOperationException_ConverterFactoryReturnsConverterFactory(GetType(), typeToConvert);
        }

        return converter!;
    }
    
    public override IRenderable ConvertInner(object? value, Type targetType, ConsoleDumpOptions options)
    {
        throw new InvalidOperationException("Converting is not valid in a factory");
    }
}