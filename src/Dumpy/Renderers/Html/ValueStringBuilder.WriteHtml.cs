// ReSharper disable once CheckNamespace

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

    public void WriteNull(DumpOptions options)
    {
        WriteOpenTag("span", $"class=\"{options.CssClasses.Null}\""); // TODO set this statically in options
        Append("null");
        WriteCloseTag("span");
    }
}