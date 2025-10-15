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
            Border = TableBorder.Rounded,
            BorderStyle = new Style(Color.PaleTurquoise4)
        };

        var typeName = Markup.Escape(TypeUtil.GetName(targetType, true));
        table.Title = options.TableOptions.ShowTitles
            ? new TableTitle(typeName, new Style(decoration: Decoration.Bold))
            : null;
        table.AddColumn(new TableColumn(new Markup("[bold][olive]Property[/][/]")));
        table.AddColumn(new TableColumn(new Markup("[bold][olive]Value[/][/]")));

        foreach (var member in GetReadableMembers(targetType, options))
        {
            switch (member)
            {
                case PropertyInfo property:
                    var propValue = TypeUtil.GetPropertyValue(property, value);
                    table.AddRow(new Markup($"[bold][olive]{property.Name}[/][/]"),
                        propValue.DumpToRenderable(property.PropertyType, options));
                    break;

                case FieldInfo field:
                    var fieldValue = TypeUtil.GetFieldValue(field, value);
                    table.AddRow(new Markup($"[bold][olive]{field.Name}[/][/]"),
                        fieldValue.DumpToRenderable(field.FieldType, options));
                    break;
            }
        }

        return table;
    }
    
    protected virtual MemberInfo[] GetReadableMembers(Type targetType, ConsoleDumpOptions options)
    {
        return options.GetReadableMembers(targetType);
    }
}