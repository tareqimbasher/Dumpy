using System;
using System.Linq;
using System.Reflection;
using Dumpy.Metadata;
using Dumpy.Renderers.Html.Utils;
using Dumpy.Utils;

namespace Dumpy.Renderers.Html;

public class ObjectConverter : IGenericConverter
{
    private static ObjectConverter? _instance;

    public static ObjectConverter Instance
    {
        get { return _instance ??= new ObjectConverter(); }
    }

    public void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, DumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNull(options);
            return;
        }

        writer.WriteOpenTag("table");
        writer.WriteOpenTag("thead"); // TODO set statically

        writer.WriteOpenTag("tr", $"class=\"{options.CssClasses.TableInfoHeader}\"");
        writer.WriteOpenTag(
            "th",
            "colspan=\"2\"",
            options.AddTitleAttributes ? $"title=\"{options.TypeMetadataProvider.GetName(targetType, true)}\"" : null
        );

        writer.Append(HtmlUtil.EscapeText(options.TypeMetadataProvider.GetName(targetType)));

        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");
        writer.WriteCloseTag("thead");

        var dataMembers = options.TypeMetadataProvider.GetDataMembers(targetType);

        if (!dataMembers.IsEmpty)
        {
            WriteTBody(ref writer, value, dataMembers, options);
        }

        writer.WriteCloseTag("table");
    }

    private void WriteTBody<T>(ref ValueStringBuilder writer, T value, DataMembers dataMembers, DumpOptions options)
    {
        writer.WriteOpenTag("tbody");

        foreach (var field in dataMembers.Fields)
        {
            WriteTRow(ref writer, field.Name, field.FieldType, TypeUtil.GetFieldValue(field, value), options);
        }

        foreach (var prop in dataMembers.Properties)
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
        DumpOptions options)
    {
        writer.WriteOpenTag("tr");

        writer.WriteOpenTag(
            "th",
            options.AddTitleAttributes
                ? $"title=\"{options.TypeMetadataProvider.GetName(memberType, true)}\""
                : null);
        writer.Append(memberName);
        writer.WriteCloseTag("th");

        writer.WriteOpenTag("td");
        Dumper.DumpHtml(ref writer, memberValue, memberType, options);
        writer.WriteCloseTag("td");

        writer.WriteCloseTag("tr");
    }
}