using System;
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
        
        var table = new Table();
        var typeName = Markup.Escape(TypeUtil.GetName(targetType, true));
        table.Title = new TableTitle(typeName, new Style(decoration: Decoration.Bold));
        table.AddColumn(new TableColumn(new Markup("[bold][olive]Property[/][/]")));
        table.AddColumn(new TableColumn(new Markup("[bold][olive]Value[/][/]")));
        table.Border(TableBorder.Rounded);
        table.BorderStyle(new Style(Color.PaleTurquoise4));
        
        if (options.IncludeNonPublicMembers)
        {
            foreach (var field in TypeUtil.GetFields(targetType, options.IncludeNonPublicMembers))
            {
                var fieldValue = TypeUtil.GetFieldValue(field, value);
                table.AddRow(new Markup($"[bold][olive]{field.Name}[/][/]"), fieldValue.DumpToRenderable(field.FieldType, options));
            }
        }
        
        foreach (var prop in GetReadableProperties(targetType, options.IncludeNonPublicMembers))
        {
            var propValue = TypeUtil.GetPropertyValue(prop, value);
            table.AddRow(new Markup($"[bold][olive]{prop.Name}[/][/]"), propValue.DumpToRenderable(prop.PropertyType, options));
        }

        return table;
    }
    
    protected virtual PropertyInfo[] GetReadableProperties(Type targetType, bool includeNonPublicMembers)
    {
        return TypeUtil.GetReadableProperties(targetType, includeNonPublicMembers);
    }
}