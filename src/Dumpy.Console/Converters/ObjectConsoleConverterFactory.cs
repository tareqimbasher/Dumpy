using System;
using System.Linq;
using System.Reflection;
using Dumpy.Console.Widgets;
using Dumpy.Utils;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class ObjectConsoleConverterFactory : ConsoleConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => true;

    public override ConsoleConverter? CreateConverter(Type typeToConvert, ConsoleDumpOptions options)
    {
        var converterType = typeof(ObjectDefaultConsoleConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as ConsoleConverter;
    }
}

public class ObjectDefaultConsoleConverter<T> : ConsoleConverter<T>
{
    public override IRenderable Convert(T? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.Instance;
        }

        var table = new Table
        {
            ShowHeaders = options.TableOptions.ShowHeaders,
            ShowRowSeparators = options.TableOptions.ShowRowSeparators,
            Expand = options.TableOptions.Expand,
        };

        var typeName = Markup.Escape(TypeUtil.GetName(targetType, true));
        table.Title = options.TableOptions.ShowTitles ? new TableTitle(typeName, new Style(decoration: Decoration.Bold)) : null;
        table.AddColumn(new TableColumn(new Markup("[bold][olive]Property[/][/]")));
        table.AddColumn(new TableColumn(new Markup("[bold][olive]Value[/][/]")));
        table.Border(TableBorder.Rounded);
        table.BorderStyle(new Style(Color.PaleTurquoise4));

        if (options.IncludeNonPublicMembers)
        {
            foreach (var field in GetReadableFields(targetType, options))
            {
                var fieldValue = TypeUtil.GetFieldValue(field, value);
                table.AddRow(new Markup($"[bold][olive]{field.Name}[/][/]"),
                    fieldValue.DumpToRenderable(field.FieldType, options));
            }
        }

        foreach (var prop in GetReadableProperties(targetType, options))
        {
            var propValue = TypeUtil.GetPropertyValue(prop, value);
            table.AddRow(new Markup($"[bold][olive]{prop.Name}[/][/]"),
                propValue.DumpToRenderable(prop.PropertyType, options));
        }

        return table;
    }

    private FieldInfo[] GetReadableFields(Type targetType, ConsoleDumpOptions options)
    {
        var fields = TypeUtil.GetFields(targetType, options.IncludeNonPublicMembers);

        if (options.MemberFilter == null)
        {
            return fields;
        }

        return fields.Where(f => options.MemberFilter(f)).ToArray();
    }

    protected virtual PropertyInfo[] GetReadableProperties(Type targetType, ConsoleDumpOptions options)
    {
        var properties = TypeUtil.GetReadableProperties(targetType, options.IncludeNonPublicMembers);

        if (options.MemberFilter == null)
        {
            return properties;
        }

        return properties.Where(p => options.MemberFilter(p)).ToArray();
    }
}