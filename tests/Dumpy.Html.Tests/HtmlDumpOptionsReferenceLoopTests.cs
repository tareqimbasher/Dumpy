namespace Dumpy.Html.Tests;

public class HtmlDumpOptionsReferenceLoopTests
{
    private class Node
    {
        public string Name { get; set; } = "root";
        public Node? Next { get; set; }
        public Node? Child { get; set; }
    }

    [Fact]
    public void ReferenceLoop_Error_Throws()
    {
        var n = new Node();
        n.Next = n; // self reference
        var options = new HtmlDumpOptions
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Error
        };

        Assert.Throws<SerializationException>(() => HtmlDumper.DumpHtml(n, options));
    }

    [Fact]
    public void ReferenceLoop_Ignore_RendersNothingForLoop()
    {
        var n = new Node { Name = "A" };
        n.Next = n;
        var options = new HtmlDumpOptions
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var html = HtmlDumper.DumpHtml(n, options);
        // Should not contain cyclic class and should have at least the property header row
        Assert.DoesNotContain(CssClassOptions.DefaultCyclicReferenceCssClass, html);
    }

    [Fact]
    public void ReferenceLoop_IgnoreAndSerializeCyclicReference_WritesSpanWithClass()
    {
        var n = new Node { Name = "A" };
        n.Next = n;
        var options = new HtmlDumpOptions
        {
            ReferenceLoopHandling = ReferenceLoopHandling.IgnoreAndSerializeCyclicReference
        };

        var html = HtmlDumper.DumpHtml(n, options);
        Assert.Contains($"class=\"{CssClassOptions.DefaultCyclicReferenceCssClass}\"", html);
    }
}