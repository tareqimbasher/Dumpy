using Dumpy.Html;

namespace Dumpy;

public static class ValueStringBuilderHtmlExtensions
{
    public static void WriteOpenTag(this ref ValueStringBuilder writer, string name, params string?[] attributes)
    {
        writer.Append("<");
        writer.Append(name);

        foreach (var attribute in attributes)
        {
            if (string.IsNullOrEmpty(attribute))
            {
                continue;
            }

            writer.Append(" ");
            writer.Append(attribute);
        }

        writer.Append(">");
    }

    public static void WriteCloseTag(this ref ValueStringBuilder writer, string name)
    {
        writer.Append("</");
        writer.Append(name);
        writer.Append(">");
    }

    public static void WriteNullHtml(this ref ValueStringBuilder writer, HtmlDumpOptions options)
    {
        writer.WriteOpenTag("span", options.CssClasses.NullFormatted);
        writer.WriteNullText();
        writer.WriteCloseTag("span");
    }

    public static void WriteNullText(this ref ValueStringBuilder writer)
    {
        writer.Append("null");
    }
}