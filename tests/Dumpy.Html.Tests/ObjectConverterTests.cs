namespace Dumpy.Html.Tests;

public class ObjectHtmlConverterTests
{
    [Fact]
    public void ShouldCorrectlySerializeObject()
    {
        var html = HtmlDumper.DumpHtml(Consts.Person);

        Assert.Equal(MinimizeHtml(Consts.PersonHtmlRepresentation), html);
    }

    private static string MinimizeHtml(string html)
    {
        var xd = new System.Xml.XmlDocument();
        xd.LoadXml(html);
        return xd.OuterXml;
    }
}
