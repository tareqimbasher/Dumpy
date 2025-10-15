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
        writer.WriteOpenTagStart("thead");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.TableInfoHeader))
        {
            writer.WriteClass(options.CssClasses.TableInfoHeader);
        }

        writer.WriteOpenTag("tr");
        writer.WriteOpenTagStart("th");
        writer.WriteAttr("colspan", "2");
        writer.WriteOpenTagEnd();

        int infoHeaderRowInsertIndex = writer.Length;

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
            writer.Append("Item");
            writer.AppendInt(iItem + 1);
            writer.WriteCloseTag("th");

            writer.WriteOpenTag("td");
            HtmlDumper.DumpHtml(ref writer, item, itemType, options);
            writer.WriteCloseTag("td");

            writer.WriteCloseTag("tr");
            serializedItemCount++;
        }

        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, TypeUtil.GetName(targetType));
        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, '(');
        if (value.Length > options.MaxCollectionSerializeLength)
        {
            infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, "First ");
        }

        // TODO add a InsertInt and make zero-alloc
        infoHeaderRowInsertIndex += writer.Insert(infoHeaderRowInsertIndex, serializedItemCount.ToString());
        writer.Insert(infoHeaderRowInsertIndex, " items)");

        writer.WriteCloseTag("tbody");
        writer.WriteCloseTag("table");
    }
}