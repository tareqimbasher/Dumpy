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

        writer.WriteOpenTagStart("table");
        if (value.Length == 0 && options.CssClasses.EmptyCollection != null)
            writer.WriteClass(options.CssClasses.EmptyCollection);
        writer.WriteOpenTagEnd();

        writer.WriteOpenTagStart("thead");
        if (options.CssClasses.TableInfoHeader != null)
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

        int serializedItemCount = 0;
        if (value.Length > 0)
        {
            writer.WriteOpenTag("tbody");

            for (int iItem = 0; iItem < value.Length; iItem++)
            {
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

                if (serializedItemCount >= options.MaxCollectionItems)
                {
                    break;
                }
            }

            writer.WriteCloseTag("tbody");
        }

        // Single string allocation is cheaper than multiple Insert() calls shifting the buffer
        var header = $"{TypeUtil.GetName(targetType)}({(value.Length > options.MaxCollectionItems ? "First " : "")}{serializedItemCount} items)";
        writer.Insert(infoHeaderRowInsertIndex, header);
        
        writer.WriteCloseTag("table");
    }
}