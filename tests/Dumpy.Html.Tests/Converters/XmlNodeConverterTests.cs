using System.Xml;
using Dumpy.Html.Utils;

namespace Dumpy.Html.Tests.Converters;

public class XmlNodeConverterTests
{
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