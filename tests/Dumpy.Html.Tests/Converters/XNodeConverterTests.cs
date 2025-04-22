using System.Xml.Linq;
using Dumpy.Html.Utils;

namespace Dumpy.Html.Tests.Converters;

public class XNodeConverterTests
{
    [Fact]
    public void ConvertsCorrectly()
    {
        var doc = XDocument.Parse("<root><child id=\"1\"/>Value</root>");

        var html = HtmlDumper.DumpHtml(doc);

        var xml = HtmlUtil.EscapeText("<root><child id=\"1\" />Value</root>");
        var expected = $"<pre><code language=\"xml\">{xml}</code></pre>";
        Assert.Equal(expected, html);
    }
}