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
            return NullWidget.New(options);
        }

        int columnCount = value.Columns.Count;
        int rowCount = value.Rows.Count;

        var table = options.CreateTable();

        var showing = rowCount > options.MaxCollectionItems
            ? $" - Showing first {options.MaxCollectionItems}"
            : "";
        var headerText = (!string.IsNullOrWhiteSpace(value.TableName) ? value.TableName : "DataTable")
                         + $" (Rows = {rowCount}{showing}, Columns = {columnCount})";

        table.Title = options.Tables.ShowTitles
            ? new TableTitle(headerText, options.Styles.TableTitleText)
            : null;

        for (int i = 0; i < columnCount; i++)
        {
            table.AddColumn(
                new TableColumn(new Text(value.Columns[i].ColumnName, options.Styles.TableHeaderText)));
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