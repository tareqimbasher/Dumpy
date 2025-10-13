using System;
using System.Runtime.CompilerServices;
using Dumpy.Utils;

namespace Dumpy.Html.Converters;

public class TupleHtmlConverter : HtmlConverter<ITuple>
{
    public override void Convert(ref ValueStringBuilder writer, ITuple? value, Type targetType, HtmlDumpOptions options)
    {
        if (value is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        writer.WriteOpenTag("table");
        writer.WriteOpenTag("thead", options.CssClasses.TableInfoHeaderFormatted);

        writer.WriteOpenTag("tr");
        writer.WriteOpenTag("th", "colspan=\"2\"");

        int infoHeaderRowStartIndex = writer.Length;

        writer.WriteCloseTag("th");
        writer.WriteCloseTag("tr");
        writer.WriteCloseTag("thead");

        writer.WriteOpenTag("tbody");

        int serializedItemCount = 0;
        for (int iItem = 0; iItem < value.Length; iItem++)
        {
            if (iItem + 1 == options.MaxCollectionSerializeLength)
            {
                break;
            }

            var item = value[iItem];
            var itemType = item == null ? typeof(object) : item.GetType();

            writer.WriteOpenTag("tr");

            writer.WriteOpenTag("th");
            writer.Append($"Item{iItem + 1}");
            writer.WriteCloseTag("th");

            writer.WriteOpenTag("td");
            HtmlDumper.DumpHtml(ref writer, item, itemType, options);
            writer.WriteCloseTag("td");

            writer.WriteCloseTag("tr");
            serializedItemCount++;
        }

        var infoHeaderText =
            $"{TypeUtil.GetName(targetType)} ({(value.Length > options.MaxCollectionSerializeLength ? "First " : "")}{serializedItemCount} items)";
        writer.Insert(infoHeaderRowStartIndex, infoHeaderText);

        writer.WriteCloseTag("tbody");
        writer.WriteCloseTag("table");
    }
}