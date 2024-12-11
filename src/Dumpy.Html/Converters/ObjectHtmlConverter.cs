using System;
using Dumpy.Html.Utils;
using Dumpy.Utils;

namespace Dumpy.Html.Converters;

public class ObjectHtmlConverter : IGenericHtmlConverter
{
    private static ObjectHtmlConverter? _instance;
    public static ObjectHtmlConverter Instance => _instance ??= new ObjectHtmlConverter();

    public void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options)
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

    private void WriteTBody<T>(ref ValueStringBuilder writer, T value, Type targetType, HtmlDumpOptions options)
    {
        writer.WriteOpenTag("tbody");

        if (options.IncludeNonPublicMembers)
        {
            foreach (var field in TypeUtil.GetFields(targetType, options.IncludeNonPublicMembers))
            {
                WriteTRow(ref writer, field.Name, field.FieldType, TypeUtil.GetFieldValue(field, value), options);
            }
        }

        foreach (var prop in TypeUtil.GetProperties(targetType, options.IncludeNonPublicMembers))
        {
            WriteTRow(ref writer, prop.Name, prop.PropertyType, TypeUtil.GetPropertyValue(prop, value), options);
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
        HtmlDumpSink.DumpHtml(ref writer, memberValue, memberType, options);
        writer.WriteCloseTag("td");

        writer.WriteCloseTag("tr");
    }
}