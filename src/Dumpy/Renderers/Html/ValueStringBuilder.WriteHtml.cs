// ReSharper disable once CheckNamespace

using Dumpy.Renderers.Html;

namespace Dumpy;

public ref partial struct ValueStringBuilder
{
    public void WriteOpenTag(string name, params string?[] attributes)
    {
        Append("<");
        Append(name);

        foreach (var attribute in attributes)
        {
            if (string.IsNullOrEmpty(attribute))
            {
                continue;
            }
            
            Append(" ");
            Append(attribute);
        }

        Append(">");
    }

    public void WriteCloseTag(string name)
    {
        Append("</");
        Append(name);
        Append(">");
    }

    public void WriteNull(HtmlDumpOptions options)
    {
        WriteOpenTag("span", options.CssClasses.NullFormatted);
        Append("null");
        WriteCloseTag("span");
    }
}