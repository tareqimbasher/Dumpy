using System;
using System.Collections.Generic;
using System.Data;
using Dumpy.Console.Widgets;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class DataSetConsoleConverter : ConsoleConverter<DataSet>
{
    public override IRenderable Convert(DataSet? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.Instance;
        }
        
        int tableCount = value.Tables.Count;
        
        var table = new Table
        {
            ShowHeaders = options.TableOptions.ShowHeaders,
            ShowRowSeparators = options.TableOptions.ShowRowSeparators,
            Expand = options.TableOptions.Expand,
            Border = TableBorder.Rounded,
            BorderStyle = new Style(Color.PaleTurquoise4)
        };
        
        var showing = tableCount > options.MaxCollectionItems
            ? $" - Showing first {options.MaxCollectionItems}"
            : "";
        var headerText = (!string.IsNullOrWhiteSpace(value.DataSetName) ? value.DataSetName : "DataSet")
                         + $" (Tables = {tableCount}{showing})";
        
        table.Title = options.TableOptions.ShowTitles ? new TableTitle(headerText, new Style(decoration: Decoration.Bold)) : null;

        table.AddColumn("No.");
        table.AddColumn("DataTable");
        
        var tablesToIterate = Math.Min(tableCount, options.MaxCollectionItems);
        for (int iTable = 0; iTable < tablesToIterate; iTable++)
        {
            var dataTable = value.Tables[iTable];
            var renderableRow = new List<IRenderable>();
            
            renderableRow.Add(new Markup((iTable + 1).ToString()));
            renderableRow.Add(dataTable.DumpToRenderable(options));
            
            table.AddRow(renderableRow);
        }

        return table;
    }
}