using System.Xml.Linq;

namespace Dumpy.Html.Tests.Converters;

public class XNodeConverterTests
{
    private static readonly HtmlDumpOptions _htmlDumpOptionsNoCss = new() { CssClasses = { Enabled = false } };

    [Fact]
    public void ConvertsNull()
    {
        var html = HtmlDumper.DumpHtml<XNode?>(null, _htmlDumpOptionsNoCss);

        Assert.Equal("<span>null</span>", html);
    }
    
    [Fact]
    public void ConvertsCorrectly()
    {
        var doc = XDocument.Parse("<root><child id=\"1\"/>Value</root>");

        var html = HtmlDumper.DumpHtml(doc);

        var xml = "<root><child id=\"1\" />Value</root>";
        var expected = $"<pre><code language=\"xml\">{xml}</code></pre>";
        Assert.Equal(expected, html);
    }
}