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
        
        var table = options.CreateTable();
        
        var showing = tableCount > options.MaxCollectionItems
            ? $" - Showing first {options.MaxCollectionItems}"
            : "";
        var headerText = (!string.IsNullOrWhiteSpace(value.DataSetName) ? value.DataSetName : "DataSet")
                         + $" (Tables = {tableCount}{showing})";
        
        table.Title = options.Tables.ShowTitles ? new TableTitle(headerText, options.Styles.TitleTextStyle) : null;

        table.AddColumn(new TableColumn(new Text("No.", options.Styles.HeaderTextStyle)));
        table.AddColumn(new TableColumn(new Text("DataTable", options.Styles.HeaderTextStyle)));
        
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