using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Dumpy.Html.Utils;

internal static class HtmlUtil
{
    private static readonly Regex _multipleSpacesRegex = new(" {2,}", RegexOptions.Compiled);
    
    [return: NotNullIfNotNull("text")]
    public static string? EscapeText(string? text)
    {
        if (text == null)
        {
            return null;
        }
        
        text = text
            .Replace("&", HtmlConstants.HtmlAmpersand)
            .Replace(" ", " ")
            .Replace("<", HtmlConstants.HtmlLessThan)
            .Replace(">", HtmlConstants.HtmlGreaterThan)
            .Replace("\"", HtmlConstants.HtmlQuote)
            .Replace("'", HtmlConstants.HtmlApostrophe)
            .Replace("\n", HtmlConstants.HtmlNewLine);
        
        return _multipleSpacesRegex.Replace(text, HtmlConstants.HtmlSpace);
    }
}