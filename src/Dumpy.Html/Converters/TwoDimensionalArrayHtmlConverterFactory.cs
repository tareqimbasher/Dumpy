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
        writer.WriteOpenTag("tr", options.CssClasses.TableInfoHeaderFormatted);
        // columnCount + 1 because we will an extra empty cell in the table data header
        writer.WriteOpenTag("th", $"colspan=\"{columnCount + 1}\"");
        writer.Append($"{TypeUtil.GetName(targetType)} (Rows = {rowCount}, Columns = {columnCount}) ({collectionLength} items)");
        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");

        // Write data header
        writer.WriteOpenTag("tr", options.CssClasses.TableDataHeaderFormatted);
        // First heading cell should be empty
        writer.WriteOpenTag("th");
        writer.WriteCloseTag("th");

        for (int i = 0; i < columnCount; i++)
        {
            writer.WriteOpenTag("th");
            writer.Append(i.ToString());
            writer.WriteCloseTag("th");
        }

        writer.WriteCloseTag("tr");
        writer.WriteCloseTag("thead");

        writer.WriteOpenTag("tbody");

        for (int iRow = 0; iRow < rowCount; iRow++)
        {
            writer.WriteOpenTag("tr");

            writer.WriteOpenTag("th");
            writer.Append(iRow.ToString());
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