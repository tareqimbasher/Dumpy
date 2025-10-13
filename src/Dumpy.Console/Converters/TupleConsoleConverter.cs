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

        var table = new Table
        {
            ShowHeaders = false, // Always false
            ShowRowSeparators = options.TableOptions.ShowRowSeparators,
            Expand = options.TableOptions.Expand,
            Border = TableBorder.Rounded,
            BorderStyle = new Style(Color.PaleTurquoise4)
        };
        table.AddColumn("");
        table.AddColumn("");

        int serializedItemCount = 0;
        for (int iItem = 0; iItem < value.Length; iItem++)
        {
            if (iItem + 1 == options.MaxCollectionSerializeLength)
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
            var exceededMax = value.Length > options.MaxCollectionSerializeLength;
            var items = $"{(exceededMax ? "First " : "")}{serializedItemCount} items";
            var typeName = Markup.Escape(TypeUtil.GetName(targetType, false));
            table.Title = new TableTitle($"{items} | {typeName}", new Style(decoration: Decoration.Bold | Decoration.Dim));
        }

        return table;
    }
}