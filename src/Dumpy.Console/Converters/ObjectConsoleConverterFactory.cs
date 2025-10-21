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

        var table = options.CreateTable();

        var typeName = Markup.Escape(TypeUtil.GetName(targetType, true));
        table.Title = options.TableOptions.ShowTitles
            ? new TableTitle(typeName, options.StyleOptions.TitleTextStyle)
            : null;
        table.AddColumn(new TableColumn(new Text("Property", options.StyleOptions.HeaderTextStyle)));
        table.AddColumn(new TableColumn(new Text("Value", options.StyleOptions.HeaderTextStyle)));

        foreach (var member in GetReadableMembers(targetType, options))
        {
            switch (member)
            {
                case PropertyInfo property:
                    var propValue = TypeUtil.GetPropertyValue(property, value);
                    table.AddRow(new Text(property.Name, options.StyleOptions.HeaderTextStyle),
                        propValue.DumpToRenderable(property.PropertyType, options));
                    break;

                case FieldInfo field:
                    var fieldValue = TypeUtil.GetFieldValue(field, value);
                    table.AddRow(new Text(field.Name, options.StyleOptions.HeaderTextStyle),
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