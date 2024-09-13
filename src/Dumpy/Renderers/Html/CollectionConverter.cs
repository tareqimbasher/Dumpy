using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using Dumpy.Renderers.Html.Utils;
using Dumpy.Utils;

namespace Dumpy.Renderers.Html;

public class CollectionConverter : IGenericConverter
{
    private static CollectionConverter? _instance;

    public static CollectionConverter Instance
    {
        get { return _instance ??= new CollectionConverter(); }
    }

    public void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, DumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNull(options);
            return;
        }

        var collection = value as IEnumerable ??
                         throw new Exception($"Value of type {targetType} is not an {nameof(IEnumerable)}.");

        var elementType = TypeUtil.GetCollectionElementType(targetType) ?? typeof(object);

        bool isElementObject = TypeUtil.IsObject(elementType);

        if (!isElementObject)
        {
            writer.WriteOpenTag("table");
            writer.WriteOpenTag("thead", $"class=\"{options.CssClasses.TableInfoHeader}\"");

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

                Dumper.DumpHtml(ref writer, element, elementType, options);

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
            var elementProperties = options.TypeMetadataProvider
                .GetMembers(elementType)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>()
                .ToArray();

            writer.WriteOpenTag("table");
            writer.WriteOpenTag("thead");

            // Info header
            writer.WriteOpenTag("tr", $"class=\"{options.CssClasses.TableInfoHeader}\"");
            writer.WriteOpenTag("th", $"colspan=\"{elementProperties.Length}\"");

            int infoHeaderRowStartIndex = writer.Length;

            writer.WriteCloseTag("th");
            writer.WriteCloseTag("tr");

            // Data header
            writer.WriteOpenTag("tr", $"class=\"{options.CssClasses.TableDataHeader}\"");
            foreach (var property in elementProperties)
            {
                writer.WriteOpenTag("th");
                writer.Append(HtmlUtil.EscapeText(property.Name));
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

                foreach (var property in elementProperties)
                {
                    writer.WriteOpenTag("td");
                    var propertyValue = TypeUtil.GetPropertyValue(property, element);
                    Dumper.DumpHtml(ref writer, propertyValue, property.PropertyType, options);
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
        var collectionTypeName = options.TypeMetadataProvider.GetName(collectionType);

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
                        sb.Append($"Key = {options.TypeMetadataProvider.GetName(keyType)}");
                    }
                    else
                    {
                        var properties = options.TypeMetadataProvider.GetMembers(keyType)
                            .Where(x => x.MemberType == MemberTypes.Property)
                            .Cast<PropertyInfo>()
                            .ToArray();
                        
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
                                    : options.TypeMetadataProvider.GetName(property.PropertyType);
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
