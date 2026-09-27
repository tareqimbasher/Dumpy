using System;
using System.Runtime.CompilerServices;
using Dumpy.Console.Widgets;
using Dumpy.Utils;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class TupleConsoleConverter : ConsoleConverter<ITuple>
{
    public override IRenderable Convert(ITuple? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value == null)
        {
            return NullWidget.New(options);
        }

        var table = options.CreateTable();
        table.ShowHeaders = false;
        table.AddColumn("");
        table.AddColumn("");

        var itemsToIterate = Math.Min(value.Length, options.MaxCollectionItems);

        for (int iItem = 0; iItem < itemsToIterate; iItem++)
        {
            var item = value[iItem];
            var itemType = item == null ? typeof(object) : item.GetType();

            table.AddRow(new Text($"Item{iItem + 1}"), item.DumpToRenderable(itemType, options));
        }

        if (options.Tables.ShowTitles)
        {
            var exceededMax = value.Length > options.MaxCollectionItems;
            var items = $"{(exceededMax ? "First " : "")}{itemsToIterate} items";
            var typeName = Markup.Escape(TypeUtil.GetName(targetType, false));
            table.Title = new TableTitle($"{typeName} | {items}", options.Styles.TableTitleText);
        }

        return table;
    }
}