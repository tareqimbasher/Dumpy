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
            .Replace("&", HtmlConsts.HtmlAmpersand)
            .Replace(" ", " ")
            .Replace("<", HtmlConsts.HtmlLessThan)
            .Replace(">", HtmlConsts.HtmlGreaterThan)
            .Replace("\"", HtmlConsts.HtmlQuote)
            .Replace("'", HtmlConsts.HtmlApostrophe)
            .Replace("\n", HtmlConsts.HtmlNewLine);
        
        return _multipleSpacesRegex.Replace(text, HtmlConsts.HtmlSpace);
    }
}