using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml;

namespace Dumpy.Html.Tests;

internal static class Util
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private static readonly Random _random = new();

    public static string GenerateRandomString(int length)
    {
        return new string(
            Enumerable
                .Repeat(Chars, length)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray()
        );
    }
    
    public static string MinimizeHtml(string html)
    {
        var xd = new XmlDocument();
        xd.LoadXml(html);
        return xd.OuterXml;
    }
    
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