using System;
using System.Data;

namespace Dumpy.Html.Converters;

public class DataTableHtmlConverter : HtmlConverter<DataTable>
{
    public override void Convert(
        ref ValueStringBuilder writer,
        DataTable? value,
        Type targetType,
        HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        int columnCount = value.Columns.Count;
        int rowCount = value.Rows.Count;

        writer.WriteOpenTagStart("table");
        if (rowCount == 0 && !string.IsNullOrWhiteSpace(options.CssClasses.EmptyCollection))
            writer.WriteClass(options.CssClasses.EmptyCollection);
        writer.WriteOpenTagEnd();

        writer.WriteOpenTag("thead");

        // Write info header
        writer.WriteOpenTagStart("tr");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.TableInfoHeader))
        {
            writer.WriteClass(options.CssClasses.TableInfoHeader);
        }

        writer.WriteOpenTagEnd();

        writer.WriteOpenTagStart("th");
        // columnCount + 1 because we will have an extra empty cell at the beginning of the data header
        writer.WriteIntAttr("colspan", columnCount + 1);
        writer.WriteOpenTagEnd();


        // Header text
        writer.Append(!string.IsNullOrWhiteSpace(value.TableName) ? value.TableName : "DataTable");
        writer.Append(" (Rows = ");
        writer.AppendInt(rowCount);

        if (rowCount > options.MaxCollectionItems)
        {
            writer.Append(" - Showing first ");
            writer.AppendInt(options.MaxCollectionItems);
        }

        writer.Append(", Columns = ");
        writer.AppendInt(columnCount);
        writer.Append(')');

        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");


        // Write data header
        writer.WriteOpenTagStart("tr");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.TableDataHeader))
        {
            writer.WriteClass(options.CssClasses.TableDataHeader);
        }

        writer.WriteOpenTagEnd();

        for (int i = 0; i < columnCount; i++)
        {
            writer.WriteOpenTag("th");
            writer.AppendEscapedText(value.Columns[i].ColumnName);
            writer.WriteCloseTag("th");
        }

        writer.WriteCloseTag("tr");
        writer.WriteCloseTag("thead");

        if (rowCount > 0)
        {
            // Table body
            writer.WriteOpenTag("tbody");

            var rowsToIterate = Math.Min(rowCount, options.MaxCollectionItems);

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
        }

        writer.WriteCloseTag("table");
    }
}