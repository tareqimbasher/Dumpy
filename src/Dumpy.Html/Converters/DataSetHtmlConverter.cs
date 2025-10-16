using System;
using System.Data;

namespace Dumpy.Html.Converters;

public class DataSetHtmlConverter : HtmlConverter<DataSet>
{
    public override void Convert(
        ref ValueStringBuilder writer,
        DataSet? value,
        Type targetType,
        HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        int tableCount = value.Tables.Count;

        writer.WriteOpenTagStart("table");
        if (tableCount == 0 && !string.IsNullOrWhiteSpace(options.CssClasses.EmptyCollection))
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
        writer.WriteAttr("colspan", "2");
        writer.WriteOpenTagEnd();

        // Header text
        writer.Append(!string.IsNullOrWhiteSpace(value.DataSetName) ? value.DataSetName : "DataSet");
        writer.Append(" (Tables = ");
        writer.AppendInt(tableCount);

        if (tableCount > options.MaxCollectionItems)
        {
            writer.Append(" - Showing first ");
            writer.AppendInt(options.MaxCollectionItems);
        }

        writer.Append(')');

        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");
        writer.WriteCloseTag("thead");

        if (tableCount > 0)
        {
            writer.WriteOpenTag("tbody");
            var tablesToIterate = Math.Min(tableCount, options.MaxCollectionItems);

            for (int iTable = 0; iTable < tablesToIterate; iTable++)
            {
                var table = value.Tables[iTable];
                writer.WriteOpenTag("tr");

                writer.WriteOpenTag("td");
                writer.AppendInt(iTable + 1);
                writer.WriteCloseTag("td");
                
                writer.WriteOpenTag("td");
                HtmlDumper.DumpHtml(ref writer, table, typeof(DataTable), options);
                writer.WriteCloseTag("td");

                writer.WriteCloseTag("tr");
            }
            
            writer.WriteCloseTag("tbody");
        }
        
        writer.WriteCloseTag("table");
    }
}