using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Dumpy.Utils;

namespace Dumpy.Html.Converters;

public class EnumerableHtmlConverterFactory : HtmlConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return TypeUtil.IsCollection(typeToConvert);
    }

    public override HtmlConverter? CreateConverter(Type typeToConvert, HtmlDumpOptions options)
    {
        var converterType = typeof(EnumerableDefaultHtmlConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as HtmlConverter;
    }
}

public class EnumerableDefaultHtmlConverter<T> : HtmlConverter<T>
{
    public override void Convert(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        var collection = value as IEnumerable ??
                         throw new SerializationException(
                             $"Value of type {targetType} is not an {nameof(IEnumerable)}.");

        var elementType = TypeUtil.GetCollectionElementType(targetType) ?? typeof(object);

        // Rendering differs depending on the type of the collection's elements:
        // Objects:
        //    "Objects" should be rendered "horizontally". Each table row represents an object from the collection,
        //    each property should have its own column and each cell contains the value of the corresponding property.
        //
        // Everything else:
        //    Each row represents an item from the collection. Each table row has only one cell with the rendered item.

        if (TypeUtil.IsObject(elementType))
        {
            ConvertObject(ref writer, collection, elementType, targetType, options);
        }
        else
        {
            ConvertNonObject(ref writer, collection, elementType, targetType, options);
        }
    }

    private void ConvertObject(
        ref ValueStringBuilder writer,
        IEnumerable collection,
        Type elementType,
        Type originalValueTargetType,
        HtmlDumpOptions options
    )
    {
        var fields = options.IncludeFields
            ? TypeUtil.GetFields(elementType, options.IncludeNonPublicMembers)
            : [];

        var properties = TypeUtil.GetReadableProperties(elementType, options.IncludeNonPublicMembers);

        writer.WriteOpenTag("table");
        writer.WriteOpenTag("thead");

        // Info header
        writer.WriteOpenTagStart("tr");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.TableInfoHeader))
        {
            writer.WriteClass(options.CssClasses.TableInfoHeader);
        }

        writer.WriteOpenTagEnd();

        writer.WriteOpenTagStart("th");
        writer.WriteIntAttr("colspan", fields.Length + properties.Length);
        writer.WriteOpenTagEnd();

        int infoHeaderRowInsertIndex = writer.Length;

        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");

        // Data header
        writer.WriteOpenTagStart("tr");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.TableDataHeader))
        {
            writer.WriteClass(options.CssClasses.TableDataHeader);
        }

        writer.WriteOpenTagEnd();

        foreach (var name in fields.Select(x => x.Name).Concat(properties.Select(x => x.Name)))
        {
            writer.WriteOpenTag("th");
            writer.AppendEscapedText(name);
            writer.WriteCloseTag("th");
        }

        writer.WriteCloseTag("tr");

        writer.WriteCloseTag("thead");
        writer.WriteOpenTag("tbody");

        int? maxCount = options.MaxCollectionSerializeLength;
        int count = 0;
        bool elementsCountExceedMax = false;

        foreach (var element in collection)
        {
            count++;

            if (count > maxCount)
            {
                elementsCountExceedMax = true;
                break;
            }

            writer.WriteOpenTag("tr");

            foreach (var field in fields) // TODO check if needs to be included
            {
                writer.WriteOpenTagStart("td");
                if (options.AddTitleAttributes)
                {
                    writer.WriteAttr("title", TypeUtil.GetName(field.FieldType, true));
                }

                writer.WriteOpenTagEnd();

                var val = TypeUtil.GetFieldValue(field, element);
                HtmlDumper.DumpHtml(ref writer, val, field.FieldType, options);
                writer.WriteCloseTag("td");
            }

            foreach (var property in properties)
            {
                writer.WriteOpenTagStart("td");
                if (options.AddTitleAttributes)
                {
                    writer.WriteAttr("title", TypeUtil.GetName(property.PropertyType, true));
                }

                writer.WriteOpenTagEnd();


                var val = TypeUtil.GetPropertyValue(property, element);
                HtmlDumper.DumpHtml(ref writer, val, property.PropertyType, options);
                writer.WriteCloseTag("td");
            }

            writer.WriteCloseTag("tr");
        }

        InsertInfoHeaderText(
            ref writer,
            infoHeaderRowInsertIndex,
            collection,
            originalValueTargetType,
            count,
            elementsCountExceedMax);

        writer.WriteCloseTag("tbody");
        writer.WriteCloseTag("table");
    }

    private void ConvertNonObject(
        ref ValueStringBuilder writer,
        IEnumerable collection,
        Type elementType,
        Type originalValueTargetType,
        HtmlDumpOptions options
    )
    {
        writer.WriteOpenTag("table");
        writer.WriteOpenTagStart("thead");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.TableInfoHeader))
        {
            writer.WriteClass(options.CssClasses.TableInfoHeader);
        }

        writer.WriteOpenTagEnd();

        writer.WriteOpenTag("tr");
        writer.WriteOpenTag("th");

        int infoHeaderRowInsertIndex = writer.Length;

        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");
        writer.WriteCloseTag("thead");

        writer.WriteOpenTag("tbody");

        int? maxCount = options.MaxCollectionSerializeLength;
        int count = 0;
        bool elementsCountExceedMax = false;

        foreach (var element in collection)
        {
            count++;

            if (count > maxCount)
            {
                elementsCountExceedMax = true;
                break;
            }

            writer.WriteOpenTag("tr");
            writer.WriteOpenTagStart("td");
            if (options.AddTitleAttributes)
            {
                writer.WriteAttr("title", TypeUtil.GetName(elementType, true));
            }

            writer.WriteOpenTagEnd();

            HtmlDumper.DumpHtml(ref writer, element, elementType, options);

            writer.WriteCloseTag("td");
            writer.WriteCloseTag("tr");
        }

        InsertInfoHeaderText(
            ref writer,
            infoHeaderRowInsertIndex,
            collection,
            originalValueTargetType,
            count,
            elementsCountExceedMax);

        writer.WriteCloseTag("tbody");
        writer.WriteCloseTag("table");
    }

    protected void InsertInfoHeaderText(
        ref ValueStringBuilder writer,
        int infoHeaderRowInsertIndex,
        IEnumerable collection,
        Type collectionType,
        int serializedElementCount,
        bool collectionHasMoreElementsThanMax)
    {
        var startingIndex = infoHeaderRowInsertIndex;
        var collectionTypeName = TypeUtil.GetName(collectionType);

        if (collectionType.Namespace == "System.Linq" && collectionTypeName.StartsWith("IGrouping<"))
        {
            var keyProp = collectionType.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public);
            if (keyProp != null)
            {
                object? key = TypeUtil.GetPropertyValue(keyProp, collection);

                if (key == null)
                {
                    infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, "Key = (null)");
                }
                else
                {
                    var keyType = key.GetType();

                    if (TypeUtil.IsStringFormattable(keyType))
                    {
                        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, "Key = ");
                        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, key.ToString());
                    }
                    else if (TypeUtil.IsCollection(keyType))
                    {
                        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, "Key = ");
                        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, TypeUtil.GetName(keyType));
                    }
                    else
                    {
                        var properties = TypeUtil.GetReadableProperties(keyType, false);

                        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, "Key = {");

                        for (var iProp = 0; iProp < properties.Length; iProp++)
                        {
                            var property = properties[iProp];
                            var propValue = TypeUtil.GetPropertyValue(property, key);
                            string? propValueStr = null;
                            if (propValue != null)
                            {
                                propValueStr = TypeUtil.IsStringFormattable(property.PropertyType)
                                    ? propValue.ToString()
                                    : TypeUtil.GetName(property.PropertyType);
                            }

                            propValueStr ??= "(null)";

                            infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, property.Name);
                            infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, ": ");
                            infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, propValueStr);

                            if ((infoHeaderRowInsertIndex - startingIndex) > 50)
                            {
                                infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, "...");
                                break;
                            }

                            if (iProp < properties.Length - 1)
                            {
                                infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, ", ");
                            }
                        }

                        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, '}');
                    }
                }

                if (infoHeaderRowInsertIndex > startingIndex)
                {
                    infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, "    ");
                }
            }
        }

        infoHeaderRowInsertIndex += writer.EscapeAndInsertText(infoHeaderRowInsertIndex, collectionTypeName);

        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, " (");
        if (collectionHasMoreElementsThanMax)
        {
            infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, "First ");
        }

        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, serializedElementCount);
        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, " items)");
    }
}