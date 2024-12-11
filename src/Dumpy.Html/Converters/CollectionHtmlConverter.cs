using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using Dumpy.Html.Utils;
using Dumpy.Utils;

namespace Dumpy.Html.Converters;

public class CollectionHtmlConverter : IGenericHtmlConverter
{
    private static CollectionHtmlConverter? _instance;
    public static CollectionHtmlConverter Instance => _instance ??= new CollectionHtmlConverter();

    public void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        var collection = value as IEnumerable ??
                         throw new Exception($"Value of type {targetType} is not an {nameof(IEnumerable)}.");

        var elementType = TypeUtil.GetCollectionElementType(targetType) ?? typeof(object);

        bool isElementObject = TypeUtil.IsObject(elementType);

        if (!isElementObject)
        {
            writer.WriteOpenTag("table");
            writer.WriteOpenTag("thead", options.CssClasses.TableInfoHeaderFormatted);

            writer.WriteOpenTag("tr");
            writer.WriteOpenTag("th");

            int infoHeaderRowStartIndex = writer.Length;

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
                writer.WriteOpenTag("td");

                HtmlDumpSink.DumpHtml(ref writer, element, elementType, options);

                writer.WriteCloseTag("td");
                writer.WriteCloseTag("tr");
            }

            var infoHeaderText = GetInfoHeaderText(collection, targetType, count, elementsCountExceedMax, options);
            writer.Insert(infoHeaderRowStartIndex, infoHeaderText);

            writer.WriteCloseTag("tbody");
            writer.WriteCloseTag("table");
        }
        else
        {
            var fields = options.IncludeFields
                ? TypeUtil.GetFields(elementType, options.IncludeNonPublicMembers)
                : Array.Empty<FieldInfo>();

            var properties = TypeUtil.GetProperties(elementType, options.IncludeNonPublicMembers);

            writer.WriteOpenTag("table");
            writer.WriteOpenTag("thead");

            // Info header
            writer.WriteOpenTag("tr", options.CssClasses.TableInfoHeaderFormatted);
            writer.WriteOpenTag("th", $"colspan=\"{fields.Length + properties.Length}\"");

            int infoHeaderRowStartIndex = writer.Length;

            writer.WriteCloseTag("th");
            writer.WriteCloseTag("tr");

            // Data header
            writer.WriteOpenTag("tr", options.CssClasses.TableDataHeaderFormatted);
            foreach (var name in fields.Select(x => x.Name).Union(properties.Select(x => x.Name)))
            {
                writer.WriteOpenTag("th");
                writer.Append(HtmlUtil.EscapeText(name));
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
                    writer.WriteOpenTag("td");
                    var val = TypeUtil.GetFieldValue(field, element);
                    HtmlDumpSink.DumpHtml(ref writer, val, field.FieldType, options);
                    writer.WriteCloseTag("td");
                }

                foreach (var property in properties)
                {
                    writer.WriteOpenTag("td");
                    var val = TypeUtil.GetPropertyValue(property, element);
                    HtmlDumpSink.DumpHtml(ref writer, val, property.PropertyType, options);
                    writer.WriteCloseTag("td");
                }

                writer.WriteCloseTag("tr");
            }

            var infoHeaderText = GetInfoHeaderText(collection, targetType, count, elementsCountExceedMax, options);
            writer.Insert(infoHeaderRowStartIndex, infoHeaderText);

            writer.WriteCloseTag("tbody");
            writer.WriteCloseTag("table");
        }
    }

    protected string GetInfoHeaderText(
        IEnumerable collection,
        Type collectionType,
        int serializedElementCount,
        bool collectionHasMoreElementsThanMax,
        DumpOptions options)
    {
        var sb = new StringBuilder();
        var collectionTypeName = TypeUtil.GetName(collectionType);

        if (collectionType.Namespace == "System.Linq" && collectionTypeName.StartsWith("IGrouping<"))
        {
            var keyProp = collectionType.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public);
            if (keyProp != null)
            {
                object? key = TypeUtil.GetPropertyValue(keyProp, collection);

                if (key == null)
                {
                    sb.Append("Key = (null)");
                }
                else
                {
                    var keyType = key.GetType();

                    if (TypeUtil.IsStringFormattable(keyType))
                    {
                        sb.Append($"Key = {key}");
                    }
                    else if (TypeUtil.IsCollection(keyType))
                    {
                        sb.Append($"Key = {TypeUtil.GetName(keyType)}");
                    }
                    else
                    {
                        var properties = TypeUtil.GetProperties(keyType, false);

                        sb.Append("Key = {");

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

                            sb.Append($"{property.Name}: {propValueStr}");

                            if (sb.Length > 50)
                            {
                                sb.Append("...");
                                break;
                            }

                            if (iProp < properties.Length - 1)
                            {
                                sb.Append(", ");
                            }
                        }

                        sb.Append('}');
                    }
                }

                if (sb.Length > 0)
                {
                    sb.Append("    ");
                }
            }
        }

        sb.Append(
            $"{HtmlUtil.EscapeText(collectionTypeName)} ({(collectionHasMoreElementsThanMax ? "First " : "")}{serializedElementCount} items)"
        );

        return sb.ToString();
    }
}