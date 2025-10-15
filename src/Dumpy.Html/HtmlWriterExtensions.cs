using System;
using System.Runtime.CompilerServices;

namespace Dumpy.Html;

public static class HtmlWriterExtensions
{
    /// <summary>
    /// Appends a full opening tag to the writer. ex: &lt;div&gt;
    /// </summary>
    /// <param name="writer">The writer to append to.</param>
    /// <param name="name">The tag name.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteOpenTag(this ref ValueStringBuilder writer, ReadOnlySpan<char> name)
    {
        writer.Append('<');
        writer.Append(name);
        writer.Append('>');
    }

    /// <summary>
    /// Appends the start of an opening tag to the writer; the angle bracket and tag name. It does not close the
    /// start tag. ex: &lt;div
    /// </summary>
    /// <param name="writer">The writer to append to.</param>
    /// <param name="name">The tag name.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteOpenTagStart(this ref ValueStringBuilder writer, ReadOnlySpan<char> name)
    {
        writer.Append('<');
        writer.Append(name);
    }

    /// <summary>
    /// Appends an attribute (name="value") to the writer. ex: title="My title"
    /// </summary>
    /// <param name="writer">The writer to append to.</param>
    /// <param name="name">The attribute name.</param>
    /// <param name="value">The attribute value.</param>
    /// <param name="escape">If true, will HTML escape the value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteAttr(
        this ref ValueStringBuilder writer,
        ReadOnlySpan<char> name,
        ReadOnlySpan<char> value,
        bool escape = true)
    {
        writer.Append(' ');
        writer.Append(name);
        writer.Append("=\"");
        if (escape) EscapeAndAppendAttributeText(ref writer, value);
        else writer.Append(value);
        writer.Append('"');
    }

    /// <summary>
    /// Appends an attribute (name="value") to the writer and sets the value to the provided
    /// <see cref="int"/> value. ex: colspan="2"
    /// </summary>
    /// <remarks>
    /// This is a convenience method for when the value is an <see cref="int"/> instead of a <see cref="string"/>. 
    /// </remarks>
    /// <param name="writer">The writer to append to.</param>
    /// <param name="name">The attribute name.</param>
    /// <param name="value">The <see cref="int"/> value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteIntAttr(this ref ValueStringBuilder writer, ReadOnlySpan<char> name, int value)
    {
        // Directly formats the integer into a small stack buffer to avoid allocating a string
        Span<char> buf = stackalloc char[11]; // enough for int32 including the '-'
        if (value.TryFormat(buf, out int written))
        {
            writer.Append(' ');
            writer.Append(name);
            writer.Append("=\"");
            writer.Append(buf.Slice(0, written));
            writer.Append('"');
        }
        else
        {
            writer.WriteAttr(name, value.ToString(), escape: false);
        }
    }

    /// <summary>
    /// Appends an attribute name to the writer and does not append a value. ex: disabled, required.
    /// </summary>
    /// <param name="writer">The writer to append to.</param>
    /// <param name="name">The attribute name.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteValuelessAttr(this ref ValueStringBuilder writer, ReadOnlySpan<char> name)
    {
        writer.Append(' ');
        writer.Append(name);
    }

    /// <summary>
    /// Appends a class attribute and value to the writer. ex: class="text-center"
    /// </summary>
    /// <param name="writer">The writer to append to.</param>
    /// <param name="value">The value of the class attribute. ex: "text-center"</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteClass(this ref ValueStringBuilder writer, ReadOnlySpan<char> value)
        => WriteAttr(ref writer, "class".AsSpan(), value);

    /// <summary>
    /// Appends a closing angle bracket (>) to the writer. 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteOpenTagEnd(this ref ValueStringBuilder writer)
        => writer.Append('>');

    /// <summary>
    /// Appends a self-closing slash and angle bracket (/>) to the writer. 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSelfClosingEnd(this ref ValueStringBuilder writer)
        => writer.Append("/>");

    /// <summary>
    /// Appends a closing tag to the writer. 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteCloseTag(this ref ValueStringBuilder writer, ReadOnlySpan<char> name)
    {
        writer.Append("</");
        writer.Append(name);
        writer.Append('>');
    }

    /// <summary>
    /// Escapes and appends the specified text to the writer.
    /// </summary>
    /// <remarks>Use when appending generic text content.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendEscapedText(this ref ValueStringBuilder writer, ReadOnlySpan<char> text)
        => EscapeAndAppendText(ref writer, text);

    /// <summary>
    /// Escapes and appends the specified attribute value text to the writer.
    /// </summary>
    /// <remarks>Use when appending an attribute's value.</remarks>
    /// <param name="writer">The writer to append to.</param>
    /// <param name="value">The value of the attribute.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendEscapedAttrValue(this ref ValueStringBuilder writer, ReadOnlySpan<char> value)
        => EscapeAndAppendAttributeText(ref writer, value);

    /// <summary>
    /// A convenience method that checks if the name and value are both not empty before appending them to the writer.
    /// </summary>
    /// <param name="writer">The writer to append to.</param>
    /// <param name="name">The attribute name.</param>
    /// <param name="value">The attribute value.</param>
    /// <param name="escape">Whether to HTML escape the attribute value or not.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteAttrIfNotEmpty(
        this ref ValueStringBuilder writer,
        string? name,
        string? value,
        bool escape = true)
    {
        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrEmpty(value))
            WriteAttr(ref writer, name.AsSpan(), value.AsSpan(), escape);
    }

    /// <summary>
    /// Writes a span indicating a null value.
    /// </summary>
    public static void WriteNullHtml(this ref ValueStringBuilder writer, HtmlDumpOptions options)
    {
        writer.WriteOpenTagStart("span");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.Null))
        {
            writer.WriteClass(options.CssClasses.Null);
        }

        writer.WriteOpenTagEnd();
        writer.Append("null");
        writer.WriteCloseTag("span");
    }

    /// <summary>
    /// Writes a span indicating a cyclic reference encounter.
    /// </summary>
    public static void WriteCyclicReferenceHtml(this ref ValueStringBuilder writer, HtmlDumpOptions options)
    {
        writer.WriteOpenTagStart("span");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.CyclicReference))
        {
            writer.WriteClass(options.CssClasses.CyclicReference);
        }

        writer.WriteOpenTagEnd();
        writer.Append("Cyclic reference");
        writer.WriteCloseTag("span");
    }

    /// <summary>
    /// Writes a span indicating max depth was reached.
    /// </summary>
    public static void WriteMaxDepthReachedHtml(this ref ValueStringBuilder writer, HtmlDumpOptions options)
    {
        writer.WriteOpenTagStart("span");
        if (!string.IsNullOrWhiteSpace(options.CssClasses.MaxDepthReached))
        {
            writer.WriteClass(options.CssClasses.MaxDepthReached);
        }

        writer.WriteOpenTagEnd();
        writer.Append("Max depth reached");
        writer.WriteCloseTag("span");
    }

    /// <summary>
    /// HTML escapes the provided text and appends it to the writer.
    /// </summary>
    public static void EscapeAndAppendText(ref ValueStringBuilder writer, ReadOnlySpan<char> text)
    {
        int i = 0;
        for (; i < text.Length; i++)
        {
            char c = text[i];
            if (c is '&' or '<' or '>' or '\n' or ' ')
                break;
        }

        if (i == text.Length)
        {
            writer.Append(text);
            return;
        }

        if (i > 0)
        {
            writer.Append(text.Slice(0, i));
        }

        for (; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '&': writer.Append("&amp;"); break;
                case '<': writer.Append("&lt;"); break;
                case '>': writer.Append("&gt;"); break;
                case '\n': writer.Append("<br/>"); break;
                case ' ': writer.Append(" "); break;
                default: writer.Append(text[i]); break;
            }
        }
    }

    /// <summary>
    /// HTML escapes the provided text and inserts it into the writer at the specified index.
    /// </summary>
    /// <returns>The length of text that was inserted.</returns>
    public static int EscapeAndInsertText(this ref ValueStringBuilder writer, int index, ReadOnlySpan<char> text)
    {
        var startingIndex = index;
        int i = 0;
        for (; i < text.Length; i++)
        {
            char c = text[i];
            if (c is '&' or '<' or '>' or '\n' or ' ')
                break;
        }

        if (i == text.Length)
        {
            index += writer.Insert(index, text);
            return index - startingIndex;
        }

        if (i > 0)
        {
            index += writer.Insert(index, text.Slice(0, i));
        }

        for (; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '&': index += writer.Insert(index, "&amp;"); break;
                case '<': index += writer.Insert(index, "&lt;"); break;
                case '>': index += writer.Insert(index, "&gt;"); break;
                case '\n': index += writer.Insert(index, "<br/>"); break;
                case ' ': index += writer.Insert(index, " "); break;
                default: index += writer.Insert(index, text[i]); break;
            }
        }

        return index - startingIndex;
    }

    public static void EscapeAndAppendAttributeText(this ref ValueStringBuilder writer, ReadOnlySpan<char> text)
    {
        int i = 0;
        for (; i < text.Length; i++)
        {
            char c = text[i];
            if (c is '&' or '<' or '>' or '"' or '\'' or ' ')
                break;
        }

        if (i == text.Length)
        {
            writer.Append(text);
            return;
        }

        if (i > 0) writer.Append(text.Slice(0, i));

        for (; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '&': writer.Append("&amp;"); break;
                case '<': writer.Append("&lt;"); break;
                case '>': writer.Append("&gt;"); break;
                case '"': writer.Append("&quot;"); break;
                case '\'': writer.Append("&#39;"); break;
                case ' ': writer.Append(" "); break;
                default: writer.Append(text[i]); break;
            }
        }
    }
}