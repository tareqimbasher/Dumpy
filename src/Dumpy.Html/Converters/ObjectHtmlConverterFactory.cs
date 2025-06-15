using System;
using System.Reflection;
using Dumpy.Html.Utils;
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

        writer.WriteOpenTag("tr", options.CssClasses.TableInfoHeaderFormatted);
        writer.WriteOpenTag(
            "th",
            "colspan=\"2\"",
            options.AddTitleAttributes ? $"title=\"{TypeUtil.GetName(targetType, true)}\"" : null
        );

        writer.Append(HtmlUtil.EscapeText(TypeUtil.GetName(targetType)));

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

        writer.WriteOpenTag(
            "th",
            options.AddTitleAttributes
                ? $"title=\"{TypeUtil.GetName(memberType, true)}\""
                : null);
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

    protected virtual (object? propValue, Type propValueType) GetPropertyValue<TObject>(PropertyInfo property, TObject obj)
    {
        return (TypeUtil.GetPropertyValue(property, obj), property.PropertyType);
    }
}