using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Dumpy.Console.Widgets;
using Dumpy.Utils;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

// ReSharper disable once InconsistentNaming
public class EnumerableConsoleConverterFactory : ConsoleConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return TypeUtil.IsCollection(typeToConvert);
    }

    public override ConsoleConverter? CreateConverter(Type typeToConvert, ConsoleDumpOptions options)
    {
        var converterType = typeof(EnumerableDefaultConsoleConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as ConsoleConverter;
    }
}

// ReSharper disable once InconsistentNaming
public class EnumerableDefaultConsoleConverter<T> : ConsoleConverter<T>
{
    public override IRenderable Convert(T? value, Type targetType, ConsoleDumpOptions options)
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
            var table = new Table
            {
                ShowHeaders = options.TableOptions.ShowHeaders,
                ShowRowSeparators = options.TableOptions.ShowRowSeparators,
                Expand = options.TableOptions.Expand,
                Border = TableBorder.Rounded,
                BorderStyle = new Style(Color.PaleTurquoise4)
            };
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

            // TODO only get type name if needed
            var typeName = Markup.Escape(TypeUtil.GetName(targetType, false));
            table.Title = options.TableOptions.ShowTitles
                ? new TableTitle(typeName, new Style(decoration: Decoration.Bold | Decoration.Dim))
                : null;
            table.Columns[0].Header($"{(elementsCountExceedMax ? "First " : "")}{rowCount} items");

            return table;
        }
        else
        {
            var fields = options.IncludeFields
                ? TypeUtil.GetFields(elementType, options.IncludeNonPublicMembers)
                : Array.Empty<FieldInfo>();

            var properties = TypeUtil.GetReadableProperties(elementType, options.IncludeNonPublicMembers);

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

            var table = new Table
            {
                ShowHeaders = options.TableOptions.ShowHeaders,
                ShowRowSeparators = options.TableOptions.ShowRowSeparators,
                Expand = options.TableOptions.Expand,
                Border = TableBorder.Rounded,
                BorderStyle = new Style(Color.PaleTurquoise4)
            };
            table.Title = options.TableOptions.ShowTitles
                ? new TableTitle(title, new Style(decoration: Decoration.Bold | Decoration.Dim))
                : null;

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