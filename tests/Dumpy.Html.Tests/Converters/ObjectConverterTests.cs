namespace Dumpy.Html.Tests.Converters;

public class ObjectHtmlConverterTests
{
    private static readonly HtmlDumpOptions _htmlDumpOptionsNoCss = new() { CssClasses = { Enabled = false } };
    
    [Fact]
    public void ConvertsNull()
    {
        var html = HtmlDumper.DumpHtml<object?>(null, _htmlDumpOptionsNoCss);

        Assert.Equal("<span>null</span>", html);
    }
    
    [Fact]
    public void ShouldSerializeObject()
    {
        var html = HtmlDumper.DumpHtml(Consts.Person);

        Assert.Equal(Util.MinimizeHtml(Consts.PersonObjectHtmlRepresentation), html);
    }
}