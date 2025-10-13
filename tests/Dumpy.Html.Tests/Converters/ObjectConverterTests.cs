namespace Dumpy.Html.Tests.Converters;

public class ObjectHtmlConverterTests
{
    [Fact]
    public void ShouldSerializeObject()
    {
        var html = HtmlDumper.DumpHtml(Consts.Person);

        Assert.Equal(Util.MinimizeHtml(Consts.PersonObjectHtmlRepresentation), html);
    }
}