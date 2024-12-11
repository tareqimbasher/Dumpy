using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Dumpy.Console.Widgets;
using Dumpy.Utils;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class CollectionConsoleConverter : IGenericConsoleConverter
{
    private static CollectionConsoleConverter? _instance;
    public static CollectionConsoleConverter Instance => _instance ??= new CollectionConsoleConverter();

    public IRenderable Convert<T>(T? value, Type targetType, DumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.Instance;
        }

        var collection = value as IEnumerable ??
                         throw new Exception($"Value of type {targetType} is not an {nameof(IEnumerable)}.");

        var elementType = TypeUtil.GetCollectionElementType(targetType) ?? typeof(object);

        bool isElementObject = TypeUtil.IsObject(elementType);

        if (!isElementObject)
        {
            var table = new Table();
            table.AddColumn("");

            int? maxCount = options.MaxCollectionSerializeLength;
            int rowCount = 0;
            bool elementsCountExceedMax = false;

            foreach (var element in collection)
            {
                rowCount++;

                if (rowCount > maxCount)
                {
                    elementsCountExceedMax = true;
                    break;
                }

                table.AddRow(element.DumpToRenderable(elementType, options));
            }

            var typeName = Markup.Escape(TypeUtil.GetName(targetType, false));
            table.Title = new TableTitle(typeName, new Style(decoration: Decoration.Bold | Decoration.Dim));
            table.Columns[0].Header($"{(elementsCountExceedMax ? "First " : "")}{rowCount} items");
            
            return table;
        }
        else
        {
            var fields = options.IncludeFields
                ? TypeUtil.GetFields(elementType, options.IncludeNonPublicMembers)
                : Array.Empty<FieldInfo>();

            var properties = TypeUtil.GetProperties(elementType, options.IncludeNonPublicMembers);

            int? maxCount = options.MaxCollectionSerializeLength;
            int rowCount = 0;
            bool elementsCountExceedMax = false;

            var rows = new List<List<IRenderable>>();

            foreach (var element in collection)
            {
                rowCount++;

                if (rowCount > maxCount)
                {
                    elementsCountExceedMax = true;
                    break;
                }

                var row = new List<IRenderable>();
                rows.Add(row);

                foreach (var field in fields) // TODO check if needs to be included
                {
                    var val = TypeUtil.GetFieldValue(field, element);
                    row.Add(val.DumpToRenderable(field.FieldType, options));
                }

                foreach (var property in properties)
                {
                    var val = TypeUtil.GetPropertyValue(property, element);
                    row.Add(val.DumpToRenderable(property.PropertyType, options));
                }
            }

            var typeName = Markup.Escape(TypeUtil.GetName(targetType, false));
            string title = $"{(elementsCountExceedMax ? "First " : "")}{rowCount} items | {typeName}";

            if (rowCount == 0)
            {
                return EmptyCollectionWidget.New(typeName);
            }

            var table = new Table();
            // table.Expand = true;
            table.Border(TableBorder.Rounded);
            table.BorderStyle(new Style(Color.PaleTurquoise4));
            table.ShowRowSeparators = true;
            table.Title = new TableTitle(title, new Style(decoration: Decoration.Bold | Decoration.Dim));

            foreach (var field in fields)
            {
                table.AddColumn(new TableColumn(new Markup($"[bold][olive]{field.Name}[/][/]")));
            }
            
            foreach (var property in properties)
            {
                table.AddColumn(new TableColumn(new Markup($"[bold][olive]{property.Name}[/][/]")));
            }
            
            foreach (var row in rows)
            {
                table.AddRow(row);
            }

            return table;
        }
    }
}