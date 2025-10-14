using System;
using System.Diagnostics;
using Dumpy.Utils;

namespace Dumpy.Html.Converters;

/// <summary>
/// Converts an object or value to HTML.
/// </summary>
public abstract class HtmlConverter
{
    /// <summary>
    /// Determines whether the type can be converted.
    /// </summary>
    /// <param name="typeToConvert">The type is checked whether it can be converted.</param>
    /// <returns>True if the type can be converted, false otherwise.</returns>
    public abstract bool CanConvert(Type typeToConvert);
    
    /// <summary>
    /// This is used internally to quickly determine the type being converted for HtmlConverter of T.
    /// </summary>
    internal abstract Type? TypeToConvert { get; }
    
    public abstract void ConvertInner(ref ValueStringBuilder writer, object? value, Type targetType, HtmlDumpOptions options);
}

/// <summary>
/// Converts an object or value to HTML.
/// </summary>
public abstract class HtmlConverter<T> : HtmlConverter
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
    
    public abstract void Convert(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options);
    
    public override void ConvertInner(ref ValueStringBuilder writer, object? value, Type targetType, HtmlDumpOptions options)
    {
        Convert(ref writer, (T?)value, targetType, options);
    }
}

/// <summary>
/// Supports converting several types by using a factory pattern.
/// </summary>
/// <remarks>
/// This is useful for converters supporting generics, such as a converter for <see cref="System.Collections.Generic.List{T}"/>.
/// </remarks>
public abstract class HtmlConverterFactory : HtmlConverter
{
    internal sealed override Type? TypeToConvert => null;
    
    /// <summary>
    /// Create a converter for the provided <see cref="Type"/>.
    /// </summary>
    /// <param name="typeToConvert">The <see cref="Type"/> being converted.</param>
    /// <param name="options">The <see cref="HtmlDumpOptions"/> being used.</param>
    /// <returns>
    /// An instance of a <see cref="HtmlConverter{T}"/> where T is compatible with <paramref name="typeToConvert"/>.
    /// If <see langword="null"/> is returned, a <see cref="NotSupportedException"/> will be thrown.
    /// </returns>
    public abstract HtmlConverter? CreateConverter(Type typeToConvert, HtmlDumpOptions options);
    
    internal HtmlConverter GetConverterInternal(Type typeToConvert, HtmlDumpOptions options)
    {
        Debug.Assert(CanConvert(typeToConvert));

        HtmlConverter? converter = CreateConverter(typeToConvert, options);
        if (converter == null)
        {
            ThrowHelper.ThrowInvalidOperationException_ConverterFactoryReturnsNullConverter(GetType(), typeToConvert);
        }

        if (converter is HtmlConverterFactory)
        {
            ThrowHelper.ThrowInvalidOperationException_ConverterFactoryReturnsConverterFactory(GetType(), typeToConvert);
        }

        return converter!;
    }
    
    public override void ConvertInner(ref ValueStringBuilder writer, object? value, Type targetType, HtmlDumpOptions options)
    {
        throw new InvalidOperationException("Converting is not valid in a factory");
    }
}