using System;
using Dumpy.Console.Widgets;
using Dumpy.Utils;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class ObjectConsoleConverter : IGenericConsoleConverter
{
    private static ObjectConsoleConverter? _instance;
    public static ObjectConsoleConverter Instance => _instance ??= new ObjectConsoleConverter();

    public IRenderable Convert<T>(T? value, Type targetType, DumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.Instance;
        }
        
        var table = new Table();
        var typeName = Markup.Escape(TypeUtil.GetName(targetType, true));
        table.Title = new TableTitle(typeName, new Style(decoration: Decoration.Bold));
        table.AddColumn("Property");
        table.AddColumn("Value");
        
        if (options.IncludeNonPublicMembers)
        {
            foreach (var field in TypeUtil.GetFields(targetType, options.IncludeNonPublicMembers))
            {
                var fieldValue = TypeUtil.GetFieldValue(field, value);
                table.AddRow(new Markup($"[green]{field.Name}[/]"), fieldValue.DumpConsole(field.FieldType, options));
            }
        }
        
        foreach (var prop in TypeUtil.GetProperties(targetType, options.IncludeNonPublicMembers))
        {
            var propValue = TypeUtil.GetPropertyValue(prop, value);
            table.AddRow(new Markup($"[green]{prop.Name}[/]"), propValue.DumpConsole(prop.PropertyType, options));
        }

        return table;
    }
}