using System;
using System.Collections.Generic;
using System.Data;
using Dumpy.Console.Widgets;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class DataTableConsoleConverter : ConsoleConverter<DataTable>
{
    public override IRenderable Convert(DataTable? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.Instance;
        }
        
        int columnCount = value.Columns.Count;
        int rowCount = value.Rows.Count;
        
        var table = new Table
        {
            ShowHeaders = options.TableOptions.ShowHeaders,
            ShowRowSeparators = options.TableOptions.ShowRowSeparators,
            Expand = options.TableOptions.Expand,
            Border = TableBorder.Rounded,
            BorderStyle = new Style(Color.PaleTurquoise4)
        };
        
        var showing = options.MaxCollectionItems > rowCount
            ? $" - Showing first {options.MaxCollectionItems}"
            : "";
        var headerText = (!string.IsNullOrWhiteSpace(value.TableName) ? value.TableName : "DataTable")
                         + $" (Rows = {rowCount}{showing}, Columns = {columnCount})";
        
        table.Title = options.TableOptions.ShowTitles ? new TableTitle(headerText, new Style(decoration: Decoration.Bold)) : null;
        
        for (int i = 0; i < columnCount; i++)
        {
            table.AddColumn(value.Columns[i].ColumnName);
        }
        
        var rowsToIterate = Math.Min(rowCount, options.MaxCollectionItems);

        for (int iRow = 0; iRow < rowsToIterate; iRow++)
        {
            var row = value.Rows[iRow];
            var renderableRow = new List<IRenderable>();

            for (int iCol = 0; iCol < columnCount; iCol++)
            {
                var item = row[iCol];
                var itemType = item == null ? typeof(object) : item.GetType();
                renderableRow.Add(ConsoleDumper.DumpToRenderable(item, itemType, options));
            }

            table.AddRow(renderableRow);
        }

        return table;
    }
}