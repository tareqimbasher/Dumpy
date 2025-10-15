using System;
using Dumpy.Utils;

namespace Dumpy.Html.Converters;

public class TwoDimensionalArrayHtmlConverterFactory : HtmlConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsArray && typeToConvert.GetArrayRank() == 2;
    }

    public override HtmlConverter? CreateConverter(Type typeToConvert, HtmlDumpOptions options)
    {
        var converterType = typeof(TwoDimensionalArrayHtmlConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as HtmlConverter;
    }
}

public class TwoDimensionalArrayHtmlConverter<T> : HtmlConverter<T>
{
    public override void Convert(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        if (value is not Array { Rank: 2 } array)
        {
            throw new SerializationException($"Value of type {targetType} is not a 2D array");
        }

        int collectionLength = array.Length;
        int rowCount = array.GetLength(0);
        int columnCount = array.GetLength(1);

        writer.WriteOpenTag("table");
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
        
        writer.AppendEscapedText(TypeUtil.GetName(targetType));
        writer.Append("(Rows = ");
        writer.AppendInt(rowCount);
        writer.Append(", Columns = ");
        writer.AppendInt(columnCount);
        writer.Append(") (");
        writer.AppendInt(collectionLength);
        writer.Append(" items)");
        
        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");
        
        
        
        // Write data header
        writer.WriteOpenTagStart("tr");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.TableDataHeader))
        {
            writer.WriteClass(options.CssClasses.TableDataHeader);
        }
        writer.WriteOpenTagEnd();
        
        // First heading cell should be empty
        writer.WriteOpenTag("th");
        writer.WriteCloseTag("th");
       
        for (int i = 0; i < columnCount; i++)
        {
            writer.WriteOpenTag("th");
            writer.AppendInt(i);
            writer.WriteCloseTag("th");
        }

        writer.WriteCloseTag("tr");
        writer.WriteCloseTag("thead");

        writer.WriteOpenTag("tbody");

        for (int iRow = 0; iRow < rowCount; iRow++)
        {
            writer.WriteOpenTag("tr");

            writer.WriteOpenTag("th");
            writer.AppendInt(iRow);
            writer.WriteCloseTag("th");

            for (int iColumn = 0; iColumn < columnCount; iColumn++)
            {
                var item = array.GetValue(iRow, iColumn);
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