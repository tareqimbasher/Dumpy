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
            return NullWidget.Instance;
        }

        var table = options.CreateTable();
        table.ShowHeaders = false;
        table.AddColumn("");
        table.AddColumn("");

        int serializedItemCount = 0;
        for (int iItem = 0; iItem < value.Length; iItem++)
        {
            if (iItem + 1 == options.MaxCollectionItems)
            {
                break;
            }

            var item = value[iItem];
            var itemType = item == null ? typeof(object) : item.GetType();

            table.AddRow(new Text($"Item{iItem + 1}"), item.DumpToRenderable(itemType, options));
            serializedItemCount++;
        }

        if (options.TableOptions.ShowTitles)
        {
            var exceededMax = value.Length > options.MaxCollectionItems;
            var items = $"{(exceededMax ? "First " : "")}{serializedItemCount} items";
            var typeName = Markup.Escape(TypeUtil.GetName(targetType, false));
            table.Title = new TableTitle($"{typeName} | {items}", options.StyleOptions.TitleTextStyle);
        }

        return table;
    }
}