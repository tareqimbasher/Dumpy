namespace Dumpy.Html.Tests.Converters;

public class ObjectHtmlConverterTests
{
    [Fact]
    public void ShouldCorrectlySerializeObject()
    {
        var html = HtmlDumper.DumpHtml(Consts.Person);

        Assert.Equal(Util.MinimizeHtml(Consts.PersonHtmlRepresentation), html);
    }
}
