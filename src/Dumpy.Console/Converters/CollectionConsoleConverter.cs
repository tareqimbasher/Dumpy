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
            table.AddColumn("# Items");

            int? maxCount = options.MaxCollectionSerializeLength;
            int count = 0;
            bool elementsCountExceedMax = false;

            foreach (var element in collection)
            {
                count++;

                if (count > maxCount)
                {
                    elementsCountExceedMax = true;
                    break;
                }

                table.AddRow(element.DumpConsole(elementType, options));
            }

            if (elementsCountExceedMax)
            {
            }

            return table;
        }
        else
        {
            var fields = options.IncludeFields
                ? TypeUtil.GetFields(elementType, options.IncludeNonPublicMembers)
                : Array.Empty<FieldInfo>();

            var properties = TypeUtil.GetProperties(elementType, options.IncludeNonPublicMembers);

            int? maxCount = options.MaxCollectionSerializeLength;
            int count = 0;
            bool elementsCountExceedMax = false;

            var rows = new List<List<IRenderable>>();

            foreach (var element in collection)
            {
                count++;

                if (count > maxCount)
                {
                    elementsCountExceedMax = true;
                    break;
                }

                var row = new List<IRenderable>();
                rows.Add(row);

                foreach (var field in fields) // TODO check if needs to be included
                {
                    var val = TypeUtil.GetFieldValue(field, element);
                    row.Add(val.DumpConsole(field.FieldType, options));
                }

                foreach (var property in properties)
                {
                    var val = TypeUtil.GetPropertyValue(property, element);
                    row.Add(val.DumpConsole(property.PropertyType, options));
                }
            }

            var table = new Table();
            var typeName = Markup.Escape(TypeUtil.GetName(targetType, false));
            table.Title = new TableTitle($"{(elementsCountExceedMax ? "First " : "")}{count} items | {typeName}", new Style(decoration: Decoration.Bold));

            foreach (var field in fields)
            {
                table.AddColumn(field.Name, c => c.LeftAligned());
            }
            
            foreach (var property in properties)
            {
                table.AddColumn(property.Name, c => c.LeftAligned());
            }
            
            foreach (var row in rows)
            {
                table.AddRow(row);
            }

            return table;
        }
    }
}