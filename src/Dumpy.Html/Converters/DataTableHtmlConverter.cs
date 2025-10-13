using System;
using System.Data;
using Dumpy.Html.Utils;
using Dumpy.Utils;

namespace Dumpy.Html.Converters;

public class DataTableHtmlConverter : HtmlConverter<DataTable>
{
    public override void Convert(ref ValueStringBuilder writer, DataTable? value, Type targetType,
        HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        int columnCount = value.Columns.Count;
        int rowCount = value.Rows.Count;

        writer.WriteOpenTag("table");
        writer.WriteOpenTag("thead");

        // Write info header
        var showing = options.MaxCollectionSerializeLength > rowCount
            ? $" - Showing {options.MaxCollectionSerializeLength}"
            : "";
        var headerText = (!string.IsNullOrWhiteSpace(value.TableName) ? value.TableName : "DataTable")
                         + $" (Rows = {rowCount}{showing}, Columns = {columnCount})";

        writer.WriteOpenTag("tr", options.CssClasses.TableInfoHeaderFormatted);
        // columnCount + 1 because we will have an extra empty cell in the table data header
        writer.WriteOpenTag("th", $"colspan=\"{columnCount + 1}\"");
        writer.Append(headerText);
        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");

        // Write data header
        writer.WriteOpenTag("tr", options.CssClasses.TableDataHeaderFormatted);

        for (int i = 0; i < columnCount; i++)
        {
            writer.WriteOpenTag("th");
            writer.Append(value.Columns[i].ColumnName);
            writer.WriteCloseTag("th");
        }

        writer.WriteCloseTag("tr");
        writer.WriteCloseTag("thead");

        writer.WriteOpenTag("tbody");

        var rowsToIterate = options.MaxCollectionSerializeLength > rowCount
            ? options.MaxCollectionSerializeLength
            : rowCount;
        
        for (int iRow = 0; iRow < rowsToIterate; iRow++)
        {
            var row = value.Rows[iRow];
            writer.WriteOpenTag("tr");

            for (int iCol = 0; iCol < columnCount; iCol++)
            {
                var item = row[iCol];
                var itemType = item == null ? typeof(object) : item.GetType();

                writer.WriteOpenTag("td");
                HtmlDumper.DumpHtml(ref writer, item, itemType, options);
                writer.WriteCloseTag("td");
            }

            writer.WriteCloseTag("tr");
        }

        writer.WriteCloseTag("tbody");
        writer.WriteCloseTag("table");
    }
}