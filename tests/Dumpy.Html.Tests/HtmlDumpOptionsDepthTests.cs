namespace Dumpy.Html.Tests;

public class HtmlDumpOptionsDepthTests
{
    private class Node
    {
        public string Name { get; set; } = "root";
        public Node? Next { get; set; }
        public Node? Child { get; set; }
    }

    [Fact]
    public void MaxDepth_Reached_WritesSpanWithClass()
    {
        var n = new Node { Name = "A", Child = new Node { Name = "B", Child = new Node { Name = "C" } } };
        var options = new HtmlDumpOptions
        {
            MaxDepth = 1 // Only root allowed; children should render as max-depth marker
        };

        var html = HtmlDumper.DumpHtml(n, options);
        Assert.Contains($"class=\"{CssClassOptions.DefaultMaxDepthReachedCssClass}\"", html);
    }

    [Fact]
    public void MaxDepth_0()
    {
        var n = new Node { Name = "A", Child = new Node { Name = "B", Child = new Node { Name = "C" } } };
        string expected = $"""
                           <span class="{CssClassOptions.DefaultMaxDepthReachedCssClass}">Max depth reached</span>
                           """;
        var options = new HtmlDumpOptions
        {
            MaxDepth = 0
        };

        var html = HtmlDumper.DumpHtml(n, options);
        Assert.Equal(expected, html);
    }

    [Fact]
    public void MaxDepth_1_ShouldShowMaxDepthReached()
    {
        var n = new Node { Name = "A", Child = new Node { Name = "B", Child = new Node { Name = "C" } } };
        var expected = $"""
                        <table>
                            <thead>
                                <tr class="{CssClassOptions.DefaultTableInfoHeaderCssClass}">
                                    <th colspan="2">Node</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <th>Name</th>
                                    <td><span class="{CssClassOptions.DefaultMaxDepthReachedCssClass}">Max depth reached</span></td>
                                </tr>
                                <tr>
                                    <th>Next</th>
                                    <td><span class="{CssClassOptions.DefaultMaxDepthReachedCssClass}">Max depth reached</span></td>
                                </tr>
                                <tr>
                                    <th>Child</th>
                                    <td><span class="{CssClassOptions.DefaultMaxDepthReachedCssClass}">Max depth reached</span></td>
                                </tr>
                            </tbody>
                        </table>
                        """;
        var options = new HtmlDumpOptions
        {
            MaxDepth = 1
        };

        var html = HtmlDumper.DumpHtml(n, options);
        Assert.Equal(Util.MinimizeHtml(expected), html);
    }

    [Fact]
    public void MaxDepth_2_ShouldShowMaxDepthReached()
    {
        var n = new Node { Name = "A", Child = new Node { Name = "B", Child = new Node { Name = "C" } } };
        var expected = $"""
                        <table>
                            <thead>
                                <tr class="{CssClassOptions.DefaultTableInfoHeaderCssClass}">
                                    <th colspan="2">Node</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <th>Name</th>
                                    <td>A</td>
                                </tr>
                                <tr>
                                    <th>Next</th>
                                    <td><span class="{CssClassOptions.DefaultNullCssClass}">null</span></td>
                                </tr>
                                <tr>
                                    <th>Child</th>
                                    <td>
                                        <table>
                                            <thead>
                                                <tr class="{CssClassOptions.DefaultTableInfoHeaderCssClass}">
                                                    <th colspan="2">Node</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <th>Name</th>
                                                    <td><span class="{CssClassOptions.DefaultMaxDepthReachedCssClass}">Max depth reached</span></td>
                                                </tr>
                                                <tr>
                                                    <th>Next</th>
                                                    <td><span class="{CssClassOptions.DefaultMaxDepthReachedCssClass}">Max depth reached</span></td>
                                                </tr>
                                                <tr>
                                                    <th>Child</th>
                                                    <td><span class="{CssClassOptions.DefaultMaxDepthReachedCssClass}">Max depth reached</span></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        """;
        var options = new HtmlDumpOptions
        {
            MaxDepth = 2
        };

        var html = HtmlDumper.DumpHtml(n, options);
        Assert.Equal(Util.MinimizeHtml(expected), html);
    }

    [Fact]
    public void MaxDepth_4_ShouldNotShowMaxDepthReached()
    {
        var n = new Node { Name = "A", Child = new Node { Name = "B", Child = new Node { Name = "C" } } };
        var options = new HtmlDumpOptions
        {
            MaxDepth = 4 // Should be enough to serialize the whole object without max depth being reached
        };

        var html = HtmlDumper.DumpHtml(n, options);
        Assert.DoesNotContain($"class=\"{CssClassOptions.DefaultMaxDepthReachedCssClass}\"", html);
    }
}