using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using Dumpy.Html.Utils;

namespace Dumpy.Benchmark;

[MemoryDiagnoser]
public class StringReplace
{
    private static readonly string Data = "<div class=\"d-block\">Hello  <span>World</span> & and ant's pants.\n</div>";
    
    [Benchmark]
    public string Replace()
    {
        return Data
            .Replace("&", HtmlConstants.HtmlAmpersand)
            .Replace(" ", HtmlConstants.HtmlSpace)
            .Replace("<", HtmlConstants.HtmlLessThan)
            .Replace(">", HtmlConstants.HtmlGreaterThan)
            .Replace("\"", HtmlConstants.HtmlQuote)
            .Replace("'", HtmlConstants.HtmlApostrophe)
            .Replace("\n", HtmlConstants.HtmlNewLine);
    }
    
    [Benchmark]
    public string StringBuilder()
    {
        return new StringBuilder(Data)
            .Replace("&", HtmlConstants.HtmlAmpersand)
            .Replace(" ", HtmlConstants.HtmlSpace)
            .Replace("<", HtmlConstants.HtmlLessThan)
            .Replace(">", HtmlConstants.HtmlGreaterThan)
            .Replace("\"", HtmlConstants.HtmlQuote)
            .Replace("'", HtmlConstants.HtmlApostrophe)
            .Replace("\n", HtmlConstants.HtmlNewLine)
            .ToString();
    }
    
    private static readonly Regex _multipleSpacesRegex = new(" {2,}", RegexOptions.Compiled);
    
    [Benchmark]
    public string Replace2()
    {
        var data = Data
            .Replace("&", HtmlConstants.HtmlAmpersand)
            .Replace("  ", HtmlConstants.HtmlSpace)
            .Replace("<", HtmlConstants.HtmlLessThan)
            .Replace(">", HtmlConstants.HtmlGreaterThan)
            .Replace("\"", HtmlConstants.HtmlQuote)
            .Replace("'", HtmlConstants.HtmlApostrophe)
            .Replace("\n", HtmlConstants.HtmlNewLine);

        return _multipleSpacesRegex.Replace(data, HtmlConstants.HtmlSpace);
    }
    
}
