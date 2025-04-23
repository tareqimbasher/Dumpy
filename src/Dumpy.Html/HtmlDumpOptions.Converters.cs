using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Dumpy.Html.Converters;
using Dumpy.Utils;

namespace Dumpy.Html;

public partial class HtmlDumpOptions
{
    private static Dictionary<Type, HtmlConverter>? _defaultSimpleConverters;
    private static HtmlConverter[]? _defaultFactoryConverters;
    private readonly ConcurrentDictionary<Type, HtmlConverter> _converters = new();

    private void RootBuiltInConverters()
    {
        if (Volatile.Read(ref _defaultFactoryConverters) is null)
        {
            _defaultSimpleConverters = GetDefaultSimpleConverters();
            Volatile.Write(ref _defaultFactoryConverters, [
                new TwoDimensionalArrayHtmlConverterFactory(),
                new StringHtmlConverterFactory(),
                // IEnumerable should always be second to last since it can convert any IEnumerable.
                new IEnumerableHtmlConverterFactory(),
                // Object should always be last since it converts any type.
                new ObjectHtmlConverterFactory(),
            ]);
        }
    }

    private static Dictionary<Type, HtmlConverter> GetDefaultSimpleConverters()
    {
        const int numberOfSimpleConverters = 2;
        var converters = new Dictionary<Type, HtmlConverter>(numberOfSimpleConverters);

        // When adding to this, update NumberOfSimpleConverters above.
        Add(BuiltInConverters.XmlNodeConverter);
        Add(BuiltInConverters.XNodeConverter);

        Debug.Assert(numberOfSimpleConverters == converters.Count);

        return converters;

        void Add(HtmlConverter converter)
        {
            Debug.Assert(converter != null);
            converters.Add(converter.TypeToConvert!, converter);
        }
    }

    public HtmlConverter GetConverter(Type typeToConvert)
    {
        if (typeToConvert == null)
        {
            throw new ArgumentNullException(nameof(typeToConvert));
        }

        RootBuiltInConverters();
        return GetConverterInternal(typeToConvert);
    }

    internal HtmlConverter GetConverterInternal(Type typeToConvert)
    {
        Debug.Assert(typeToConvert != null);

        if (_converters.TryGetValue(typeToConvert, out var converter))
        {
            Debug.Assert(converter != null);
            return converter;
        }

        // Priority 1: Attempt to get custom converter
        foreach (var item in Converters)
        {
            if (item.CanConvert(typeToConvert))
            {
                converter = item;
                break;
            }
        }

        // Priority 2: Attempt to get built-in converter
        if (converter == null)
        {
            if (_defaultSimpleConverters == null || _defaultFactoryConverters == null)
            {
                Debug.Assert(_defaultSimpleConverters == null);
                Debug.Assert(_defaultFactoryConverters == null);
                ThrowHelper.ThrowNotSupportedException_BuiltInConvertersNotRooted(typeToConvert);
            }

            // Attempt to get simple converter by exact type match
            if (_defaultSimpleConverters.TryGetValue(typeToConvert, out var simpleConverter))
            {
                converter = simpleConverter;
            }
            else
            {
                // Attempt to find a simple converter that can convert
                simpleConverter = _defaultSimpleConverters.Values.FirstOrDefault(x => x.CanConvert(typeToConvert));
                if (simpleConverter != null)
                {
                    converter = simpleConverter;
                }
                else
                {
                    // Find a suitable factory converter
                    converter = _defaultFactoryConverters.FirstOrDefault(x => x.CanConvert(typeToConvert));
                }

                // Since the object and IEnumerable converters cover all types, we should have a converter.
                Debug.Assert(converter != null);
            }
        }

        if (converter is HtmlConverterFactory factory)
        {
            converter = factory.GetConverterInternal(typeToConvert, this);
        }

        // Converter should be a simple converter that has a TypeToConvert defined
        Debug.Assert(converter != null);
        Debug.Assert(converter.TypeToConvert != null);

        var converterTypeToConvert = converter.TypeToConvert;

        if (!converterTypeToConvert.IsAssignableFromInternal(typeToConvert)
            && !typeToConvert.IsAssignableFromInternal(converterTypeToConvert))
        {
            ThrowHelper.ThrowInvalidOperationException_ConverterNotCompatible(converter.GetType(), typeToConvert);
        }

        _converters.TryAdd(typeToConvert, converter);
        return converter;
    }
}