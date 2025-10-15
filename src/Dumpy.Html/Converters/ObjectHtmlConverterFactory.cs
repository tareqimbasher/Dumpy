using System;
using System.Reflection;
using Dumpy.Utils;

namespace Dumpy.Html.Converters;

public class ObjectHtmlConverterFactory : HtmlConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => true;

    public override HtmlConverter? CreateConverter(Type typeToConvert, HtmlDumpOptions options)
    {
        var converterType = typeof(ObjectDefaultHtmlConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as HtmlConverter;
    }
}

public class ObjectDefaultHtmlConverter<T> : HtmlConverter<T>
{
    public override void Convert(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        writer.WriteOpenTag("table");
        writer.WriteOpenTag("thead"); // TODO set statically

        writer.WriteOpenTagStart("tr");
        if (options.CssClasses.TableInfoHeader != null)
        {
            writer.WriteClass(options.CssClasses.TableInfoHeader);
        }
        writer.WriteOpenTagEnd();

        writer.WriteOpenTagStart("th");
        writer.WriteAttr("colspan", "2");
        if (options.AddTitleAttributes)
        {
            writer.WriteAttr("title", TypeUtil.GetName(targetType, true));
        }

        writer.WriteOpenTagEnd();

        writer.AppendEscapedText(TypeUtil.GetName(targetType));

        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");
        writer.WriteCloseTag("thead");

        WriteTBody(ref writer, value, targetType, options);

        writer.WriteCloseTag("table");
    }

    private void WriteTBody(ref ValueStringBuilder writer, T value, Type targetType, HtmlDumpOptions options)
    {
        writer.WriteOpenTag("tbody");

        if (options.IncludeFields)
        {
            foreach (var field in TypeUtil.GetFields(targetType, options.IncludeNonPublicMembers))
            {
                WriteTRow(ref writer, field.Name, field.FieldType, TypeUtil.GetFieldValue(field, value), options);
            }
        }

        foreach (var prop in GetReadableProperties(targetType, options))
        {
            var (propValue, propValueType) = GetPropertyValue(prop, value);
            WriteTRow(ref writer, prop.Name, propValueType, propValue, options);
        }

        writer.WriteCloseTag("tbody");
    }

    private void WriteTRow(
        ref ValueStringBuilder writer,
        string memberName,
        Type memberType,
        object? memberValue,
        HtmlDumpOptions options)
    {
        writer.WriteOpenTag("tr");

        writer.WriteOpenTagStart("th");
        if (options.AddTitleAttributes)
        {
            writer.WriteAttr("title", TypeUtil.GetName(memberType, true));
        }

        writer.WriteOpenTagEnd();

        writer.Append(memberName);
        writer.WriteCloseTag("th");

        writer.WriteOpenTag("td");
        HtmlDumper.DumpHtml(ref writer, memberValue, memberType, options);
        writer.WriteCloseTag("td");

        writer.WriteCloseTag("tr");
    }

    protected virtual PropertyInfo[] GetReadableProperties(Type targetType, HtmlDumpOptions options)
    {
        return TypeUtil.GetReadableProperties(targetType, options.IncludeNonPublicMembers);
    }

    protected virtual (object? propValue, Type propValueType) GetPropertyValue<TObject>(PropertyInfo property,
        TObject obj)
    {
        return (TypeUtil.GetPropertyValue(property, obj), property.PropertyType);
    }
}