using System.Xml;
using Dumpy.Html.Utils;

namespace Dumpy.Html.Tests.Converters;

public class XmlNodeConverterTests
{
    private static readonly HtmlDumpOptions _htmlDumpOptionsNoCss = new() { CssClasses = { Enabled = false } };

    [Fact]
    public void ConvertsNull()
    {
        var html = HtmlDumper.DumpHtml<XmlNode?>(null, _htmlDumpOptionsNoCss);

        Assert.Equal("<span>null</span>", html);
    }
    
    [Fact]
    public void ConvertsCorrectly()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><child id=\"1\"/>Value</root>");

        var html = HtmlDumper.DumpHtml(doc, typeof(XmlNode));

        var xml = HtmlUtil.EscapeText("<root><child id=\"1\" />Value</root>");
        var expected = $"<pre><code language=\"xml\">{xml}</code></pre>";
        Assert.Equal(expected, html);
    }
}