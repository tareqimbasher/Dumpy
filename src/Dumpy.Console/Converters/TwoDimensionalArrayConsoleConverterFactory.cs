using System;
using System.Collections.Generic;
using Dumpy.Console.Widgets;
using Dumpy.Utils;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class TwoDimensionalArrayConsoleConverterFactory : ConsoleConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsArray && typeToConvert.GetArrayRank() == 2;
    }

    public override ConsoleConverter? CreateConverter(Type typeToConvert, ConsoleDumpOptions options)
    {
        var converterType = typeof(TwoDimensionalArrayConsoleConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as ConsoleConverter;
    }
}

public class TwoDimensionalArrayConsoleConverter<T> : ConsoleConverter<T>
{
    public override IRenderable Convert(T? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.Instance;
        }

        if (value is not Array { Rank: 2 } array)
        {
            throw new SerializationException($"Value of type {targetType} is not a 2D array");
        }

        int collectionLength = array.Length;
        int rowCount = array.GetLength(0);
        int columnCount = array.GetLength(1);
        var typeName = Markup.Escape(TypeUtil.GetName(targetType));

        if (rowCount == 0)
        {
            return EmptyCollectionWidget.New(typeName);
        }

        var rowsToIterate = Math.Min(rowCount, options.MaxCollectionItems);

        var table = options.CreateTable();

        if (options.Tables.ShowTitles)
        {
            var exceededMax = rowCount > options.MaxCollectionItems;
            var items = $"{(exceededMax ? "First " : "")}{rowsToIterate} items";
            table.Title = new TableTitle($"{typeName} | {items}", options.Styles.TitleTextStyle);
        }

        // Add an empty column at the start
        table.AddColumn("");

        for (int i = 0; i < columnCount; i++)
        {
            table.AddColumn(new TableColumn(new Text(i.ToString(), options.Styles.HeaderTextStyle)));
        }

        for (int iRow = 0; iRow < rowsToIterate; iRow++)
        {
            var row = new List<IRenderable> { new Markup(iRow.ToString()) };

            for (int iColumn = 0; iColumn < columnCount; iColumn++)
            {
                var item = array.GetValue(iRow, iColumn);
                var itemType = item == null ? typeof(object) : item.GetType();
                row.Add(ConsoleDumper.DumpToRenderable(item, itemType, options));
            }

            table.AddRow(row);
        }

        return table;
    }
}