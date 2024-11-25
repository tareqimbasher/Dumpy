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
            .Replace("&", HtmlConsts.HtmlAmpersand)
            .Replace(" ", HtmlConsts.HtmlSpace)
            .Replace("<", HtmlConsts.HtmlLessThan)
            .Replace(">", HtmlConsts.HtmlGreaterThan)
            .Replace("\"", HtmlConsts.HtmlQuote)
            .Replace("'", HtmlConsts.HtmlApostrophe)
            .Replace("\n", HtmlConsts.HtmlNewLine);
    }
    
    [Benchmark]
    public string StringBuilder()
    {
        return new StringBuilder(Data)
            .Replace("&", HtmlConsts.HtmlAmpersand)
            .Replace(" ", HtmlConsts.HtmlSpace)
            .Replace("<", HtmlConsts.HtmlLessThan)
            .Replace(">", HtmlConsts.HtmlGreaterThan)
            .Replace("\"", HtmlConsts.HtmlQuote)
            .Replace("'", HtmlConsts.HtmlApostrophe)
            .Replace("\n", HtmlConsts.HtmlNewLine)
            .ToString();
    }
    
    private static readonly Regex _multipleSpacesRegex = new(" {2,}", RegexOptions.Compiled);
    
    [Benchmark]
    public string Replace2()
    {
        var data = Data
            .Replace("&", HtmlConsts.HtmlAmpersand)
            .Replace("  ", HtmlConsts.HtmlSpace)
            .Replace("<", HtmlConsts.HtmlLessThan)
            .Replace(">", HtmlConsts.HtmlGreaterThan)
            .Replace("\"", HtmlConsts.HtmlQuote)
            .Replace("'", HtmlConsts.HtmlApostrophe)
            .Replace("\n", HtmlConsts.HtmlNewLine);

        return _multipleSpacesRegex.Replace(data, HtmlConsts.HtmlSpace);
    }
    
}
